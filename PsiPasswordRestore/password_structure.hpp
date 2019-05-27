#pragma once

#include <array>
#include <memory>
#include <vector>

#include "hash_structure.hpp"
#include "string_structure.hpp"

namespace psi {

struct password_t
{
	uint64_t id;
	hash_t<16> nt;
	hash_t<16> lm;
	string_t plaintext;
	bool state;

public:
	password_t() = delete;
	password_t(uint64_t id, std::vector<uint8_t> const& nt, std::vector<uint8_t> const& lm);

	bool compare_nt(std::array<uint8_t, 16> const& hash) const;
	bool compare_lm(std::array<uint8_t, 16> const& hash) const;

	bool resolved() const;
	void resolve(std::string const& plaintext);
};

} // namespace psi