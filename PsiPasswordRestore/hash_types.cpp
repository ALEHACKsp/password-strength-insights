#include "hash_types.hpp"
#include "string_cast.hpp"

#include "md4.hpp"

namespace psi {

std::array<uint8_t, 16> md4_hash_format::operator()(void const* data, std::size_t size) const
{
	std::array<uint8_t, 16> a = { 0 };

	MD4_CTX ctx;
	MD4_Init(&ctx);
	MD4_Update(&ctx, data, static_cast<unsigned long>(size));
	MD4_Final(a.data(), &ctx);

	return a;
}

std::array<uint8_t, 16> nt_hash_format::operator()(void const* data, std::size_t size) const
{
	char const* begin = reinterpret_cast<char const*>(data);
	std::wstring unicode = string_cast<std::wstring>(std::string(begin, begin + size));

	return md4_hash_format()(unicode.data(), unicode.length() * sizeof(std::wstring::value_type));
}

} // namespace psi