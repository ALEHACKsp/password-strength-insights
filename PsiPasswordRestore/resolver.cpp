#include "resolver.hpp"

#include <algorithm>
#include <iostream>

namespace psi {
namespace detail {

template <class Iterator, class T>
inline std::size_t binarysearch(Iterator begin, Iterator end, T const& value)
{
	Iterator iter = std::lower_bound(begin, end, value, std::less<>());

	if (iter != end && std::less<>()(value, *iter) == 0)
		return (iter - begin);
	else
		return static_cast<std::size_t>(-1);
}

} // namespace detail

void resolver_t::test(std::vector<file_pair> const& lookups, std::vector<password_t>& passwords)
{
	/*
		We should maintain a continuous memory usage of maximum 2 MB or less for the hash-data container
		If the processing is too slow, try increasing the maximum memory usage to 1 GB (or more)
	*/
	static constexpr uint64_t usage = 512000000;

	for (file_pair const& source : lookups)
	{
		source.first.reset();

		uint64_t index = 0;
		std::cout << "index = 0 " << std::endl;

		for (uint64_t remains = source.first.size(), processed = 0; remains > 0; remains -= processed)
		{
			read(source.first, static_cast<std::size_t>(processed = std::min<uint64_t>(remains, usage)));

			for (password_t& password : passwords)
			{
				if (!password.resolved())
				{
					std::size_t i = this->find(password.nt.get());
					
					if (i != static_cast<std::size_t>(-1))
						password.resolve(source.second.read(index + i));
				}
			}

			index += this->hashes.size();
		}
	}

	this->hashes.clear();
	this->hashes.shrink_to_fit();
}

void resolver_t::read(hash_list_t const& source, std::size_t bytes)
{
	if ((bytes % 16) != 0)
		throw std::logic_error("cannot read hashes to a non-aligned buffer");
	else
	{
		this->hashes.clear();
		this->hashes.reserve(bytes / 16);

		std::vector<uint8_t> buffer;

		for (uint32_t length = 0; bytes > 0; )
		{
			std::array<uint8_t, 16 * 1024> temp;

			while ((length = source.read(&temp[0], std::min<std::size_t>(temp.size(), bytes - buffer.size()))) != 0)
			{
				buffer.insert(buffer.end(), temp.begin(), temp.begin() + length);
				std::vector<uint8_t>::const_iterator iterator = buffer.cbegin();

				while (iterator != buffer.cend() && std::distance(iterator, buffer.cend()) >= 16)
				{
					this->hashes.push_back(hash_data());
					std::copy(iterator, iterator + 16, this->hashes.back().data());

					std::advance(iterator, 16);
					bytes -= 16;
				}

				buffer.erase(buffer.begin(), iterator);
			}
		}
	}
}

std::size_t resolver_t::find(hash_data const& entry) const
{
	return detail::binarysearch(hashes.begin(), hashes.end(), entry);
}

} // namespace psi