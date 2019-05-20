#include "hash_structure.hpp"

#include <intrin.h>

namespace psi {

inline bool hash_t<16>::compare(hash_t<16> const& other) const
{
	__m128i const cmp128 = _mm_cmpeq_epi64(
		*reinterpret_cast<__m128i const*>(this->hash.data()),
		*reinterpret_cast<__m128i const*>(other.hash.data()));

	return (_mm_movemask_epi8(cmp128) == 0xffff);

	//__m128i const xor128 = _mm_xor_si128(
	//	*reinterpret_cast<__m128i const*>(this->hash.data()),
	//	*reinterpret_cast<__m128i const*>(other.hash.data()));

	//return (_mm_testz_si128(xor128, xor128));
}

} // namespace psi