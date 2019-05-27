#pragma once

#include <memory>
#include <string>

namespace psi {

class string_t
{ 
public:
	string_t();
	string_t(std::string const& text);

	string_t(string_t const& other);
	string_t(string_t&& other) = default;

	string_t& operator=(string_t const& other);
	string_t& operator=(string_t&& other) = default;

	char* str() const;
	char const* const c_str() const;

	bool empty() const;

	std::size_t bytes() const;	// Total size in bytes
	std::size_t chars() const;	// Total character count

private:
	std::unique_ptr<char[]> data;
};

} // namespace psi