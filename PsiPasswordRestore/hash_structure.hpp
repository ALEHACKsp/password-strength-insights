#pragma once

#include <array>

namespace psi {
namespace detail {

template <std::size_t Size>
class hash_base
{
public:
	constexpr hash_base(std::array<uint8_t, Size> const& data) :
		hash(data)
	{

	}

	uint8_t const* data() const
	{
		return this->hash.data();
	}

	constexpr std::size_t length() const
	{
		return this->hash.size();
	}

	constexpr bool operator<(hash_base<Size> const& other) const { return (this->hash < other.hash); }
	constexpr bool operator<=(hash_base<Size> const& other) const { return (this->hash <= other.hash); }
	constexpr bool operator>(hash_base<Size> const& other) const { return (this->hash > other.hash); }
	constexpr bool operator>=(hash_base<Size> const& other) const { return (this->hash >= other.hash); }

protected:
	std::array<uint8_t, Size> hash;
};

} // namespace detail 

template <std::size_t Size>
class hash_t : public detail::hash_base<Size>
{ 
	using detail::hash_base<Size>::hash_base;
};

template <>
class hash_t<16> : public detail::hash_base<16>
{
	using detail::hash_base<16>::hash_base;

public:
	bool compare(hash_t<16> const& other) const;
};

} // namespace psi