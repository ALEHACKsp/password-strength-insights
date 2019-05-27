#pragma once

#include <string>

namespace psi {

class plaintext_list_t
{
public:
	plaintext_list_t();
	~plaintext_list_t();

	void open(std::string const& path);
	void close();

	std::string read(uint64_t index) const;

private:
	uint32_t read(void* const buffer, std::size_t length) const;
	void point(uint64_t offset) const;

private:
	void* handle;
	uint64_t bytes;
};

} // namespace psi