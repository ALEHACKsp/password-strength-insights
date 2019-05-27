#pragma once

#include "hash_list.hpp"
#include "plaintext_list.hpp"
#include "password_structure.hpp"

#include <vector>

namespace psi {

class resolver_t
{
	using file_pair = std::pair<hash_list_t, plaintext_list_t>;
	using hash_data = std::array<uint8_t, 16>;

public:
	void test(std::vector<file_pair> const& lookups, std::vector<password_t>& passwords);

private:
	void read(hash_list_t const& source, std::size_t bytes);
	std::size_t find(hash_data const& entry) const;

private:
	std::vector<file_pair> lookups;
	std::vector<hash_data> hashes;
};

} // namespace psi