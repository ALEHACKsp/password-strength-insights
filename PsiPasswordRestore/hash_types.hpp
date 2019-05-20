#pragma once

#include <stdint.h>

#include <array>
#include <vector>

namespace psi {
namespace detail {

template <std::size_t Size>
struct hash_functor
{
	static constexpr std::size_t size = Size;

	virtual std::array<uint8_t, size> operator()(void const* data, std::size_t size) const = 0;
};

} // namespace detail

struct md4_hash_format : public detail::hash_functor<16>
{
	std::array<uint8_t, 16> operator()(void const* data, std::size_t size) const override;
};

struct nt_hash_format : public md4_hash_format
{
	std::array<uint8_t, 16> operator()(void const* data, std::size_t size) const override;
};

} // namespace psi