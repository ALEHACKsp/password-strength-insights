#pragma once

#include "string_constants.hpp"

#include <string>
#include <vector>

namespace psi {
namespace detail {
namespace convert {

int string_convert(codepage cp, std::string const& input, std::vector<wchar_t>& output);
int string_convert(codepage cp, std::wstring const& input, std::vector<char>& output);

} // namespace convert

namespace traits {

template <typename T>
struct string_by_type;

template <> struct string_by_type<char const*> { using wrap = std::string; };
template <> struct string_by_type<wchar_t const*> { using wrap = std::wstring; };

} // namespace traits

template <typename T1, typename T2>
struct string_cast_impl
{
	static inline T1 cast(T2 const& s, codepage const cp)
	{
		std::vector<typename T1::value_type> v;
		int const length = convert::string_convert(cp, s, v);

		if (length <= 0)
			return T1();
		else
		{
			v.resize(static_cast<std::size_t>(length));
			convert::string_convert(cp, s, v);

			return T1(v.begin(), v.end());
		}
	}
};

template <typename T>
struct string_cast_impl<T, T>
{
	static inline T cast(T const& s, codepage const cp)
	{
		return s;
	}
};

} // namespace detail

template <typename T1, typename T2>
T1 string_cast(T2 const& s, codepage const cp = codepage::utf8)
{
	return detail::string_cast_impl<T1, T2>::cast(s, cp);
}

template <typename T1, typename T2>
T1 string_cast(T2 const* s, codepage const cp = codepage::utf8)
{
	using Tw = typename detail::traits::string_by_type<T2 const*>::wrap;
	return detail::string_cast_impl<T1, Tw>::cast(s, cp);
}

} // namespace psi