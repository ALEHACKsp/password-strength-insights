#include "password_structure.hpp"

#include <intrin.h>

namespace psi {

password_t::password_t(uint64_t id, std::vector<uint8_t> const& nt, std::vector<uint8_t> const& lm) :
	id(id),
	plaintext(std::string()),
	state(false)
{
	if (!nt.empty() && nt.size() == 16)
	{
		this->nt = std::array<uint8_t, 16>();
		std::copy(nt.begin(), nt.end(), this->nt.get().begin());
	}

	if (!lm.empty() && lm.size() == 16)
	{
		this->lm = std::array<uint8_t, 16>();
		std::copy(lm.begin(), lm.end(), this->lm.get().begin());
	}
}

bool password_t::compare_nt(std::array<uint8_t, 16> const& hash) const
{
	return this->nt.compare(hash);
}

bool password_t::compare_lm(std::array<uint8_t, 16> const& hash) const
{
	return this->lm.compare(hash);
}

bool password_t::resolved() const
{
	return this->state;
}

void password_t::resolve(std::string const& plaintext)
{
	this->plaintext = plaintext;
	this->state = true;
}

} // namespace psi