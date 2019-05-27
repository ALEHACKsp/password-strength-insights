#pragma once

#include <string>

namespace psi {

class hash_list_t
{
public:
	hash_list_t();
	~hash_list_t();

	void open(std::string const& path);
	void close();

	void reset() const;

	uint32_t read(void* const buffer, std::size_t length) const;

	uint64_t size() const;
	uint64_t elements() const;

private:
	void* handle;
	uint64_t bytes;
	uint64_t count;
};

} // namespace psi