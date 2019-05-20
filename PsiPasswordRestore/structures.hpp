#pragma once

#include <array>
#include <memory>
#include <vector>

namespace psi {

struct password
{
	uint64_t id;
	std::array<uint8_t, 16> nt;
	std::unique_ptr<uint8_t[]> lm;
	std::string plaintext;

public:
	password() = delete;
	password(uint64_t id, std::vector<uint8_t> const& nt, std::vector<uint8_t> const& lm);

	bool compare_nt(std::array<uint8_t, 16> const& hash) const;
	bool compare_lm(std::array<uint8_t, 16> const& hash) const;

	bool resolved() const;
	void resolve(std::string const& plaintext);
};

} // namespace psi