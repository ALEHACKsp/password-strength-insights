#include "structures.hpp"

#include <intrin.h>

namespace psi {
namespace detail {

bool compare(uint8_t const* h1, uint8_t const* h2)
{
	__m128i const cmp128 = _mm_cmpeq_epi64(
		*reinterpret_cast<__m128i const*>(h1),
		*reinterpret_cast<__m128i const*>(h2));

	return (_mm_movemask_epi8(cmp128) == 0xffff);

	//__m128i const xor128 = _mm_xor_si128(
	//	*reinterpret_cast<__m128i const*>(h1),
	//	*reinterpret_cast<__m128i const*>(h2));

	//return (_mm_testz_si128(xor128, xor128));
}

} // namespace detail

password::password(uint64_t id, std::vector<uint8_t> const& nt, std::vector<uint8_t> const& lm) :
	id(id),
	plaintext(std::string())
{
	if (!nt.empty() && nt.size() == 16)
		memcpy(this->nt.data(), nt.data(), nt.size());

	if (!lm.empty() && lm.size() == 16)
	{
		this->lm = std::make_unique<uint8_t[]>(lm.size());
		memcpy(&this->lm[0], &lm[0], lm.size());
	}
}

bool password::compare_nt(std::array<uint8_t, 16> const& hash) const
{
	return detail::compare(this->nt.data(), hash.data());
}

bool password::compare_lm(std::array<uint8_t, 16> const& hash) const
{
	if (this->lm == nullptr)
		return false;
	else
		return detail::compare(&this->lm[0], hash.data());
}

bool password::resolved() const
{
	return (!this->plaintext.empty());
}

void password::resolve(std::string const& plaintext)
{
	this->plaintext = plaintext;
}

} // namespace psi