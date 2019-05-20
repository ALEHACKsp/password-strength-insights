#pragma once

#include "parser.hpp"

#include "hash_types.hpp"
#include "parser_types.hpp"

namespace psi {

class repacker_t
{
public:
	repacker_t() :
		parser()
	{

	}

	template <typename... Delims>
	repacker_t(Delims&&... delims) :
		parser(std::forward<Delims>(delims)...)
	{

	}

	void repack_file(std::string const& path)
	{
		auto pairs = parser.read_file(path);

		std::sort(pairs.begin(), pairs.end(), [&](auto const& p1, auto const& p2) -> bool
		{
			return p1.second < p2.second;
		});

		std::size_t offset = path.find_last_of('.');

		if (offset != std::string::npos)
		{
			std::string file = path.substr(0, offset);

			this->write_hashes(file + ".hl", pairs);
			this->write_plaintexts(file + ".pl", pairs);
		}
	}

	template <std::size_t Size>
	void write_hashes(std::string const& path, std::vector<std::pair<string_t, hash_t<Size>>> pairs)
	{
		std::ofstream file(path, std::ios::out | std::ios::binary);
		std::cout << "Writing to " << path << "..." << std::endl;

		for (auto const& pair : pairs)
			file.write(reinterpret_cast<char const*>(pair.second.data()), Size);

		file.flush();
		file.close();
	}

	template <std::size_t Size>
	void write_plaintexts(std::string const& path, std::vector<std::pair<string_t, hash_t<Size>>> pairs)
	{
		uint64_t offset = sizeof(uint64_t) * pairs.size();
		std::vector<uint64_t> index_list;

		for (auto const& pair : pairs)
		{
			index_list.insert(index_list.end(), offset);
			offset += pair.first.bytes();
		}

		std::ofstream file(path, std::ios::out | std::ios::binary);
		std::cout << "Writing to " << path << "..." << std::endl;

		file.write(reinterpret_cast<char const*>(index_list.data()), sizeof(uint64_t) * index_list.size());

		for (auto const& pair : pairs)
			file.write(pair.first.c_str(), pair.first.bytes());

		file.flush();
		file.close();
	}

private:
	parser_t<nt_hash_format, password_format> parser;
};

} // namespace psi