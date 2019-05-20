#include "string_structure.hpp"

#include <algorithm>

namespace psi {

string_t::string_t(std::string const& text)
{
	this->data = std::make_unique<char[]>(text.size() + 1);
	this->data[text.size()] = '\0';

	std::copy(text.begin(), text.end(), this->data.get());
}

string_t::string_t(string_t const& other)
{
	this->data = std::make_unique<char[]>(other.bytes() + 1);
	this->data[other.bytes()] = '\0';

	std::copy(other.c_str(), other.c_str() + other.bytes(), this->data.get());
}

string_t& string_t::operator=(string_t const& other)
{
	if (this != &other)
	{
		this->data = std::make_unique<char[]>(other.bytes() + 1);
		this->data[other.bytes()] = '\0';

		std::copy(other.c_str(), other.c_str() + other.bytes(), this->data.get());
	}

	return *this;
}

char const* const string_t::c_str() const
{
	return this->data.get();
}

std::size_t string_t::bytes() const
{
	char const* const start = this->c_str();
	char const* end = start;

	if (end == nullptr || (*end) == '\0')
		return 0;
	else
	{
		while ((*end) != '\0')
			end++;

		return static_cast<std::size_t>(end - start);
	}
}

std::size_t string_t::chars() const
{
	/*	# of bytes		first code		last code		byte #1		byte #2		byte #3		byte #4
						point			point

		1 (07 bits):	U+000000		U+00007f		0xxxxxxx
		2 (11 bits):	U+000080		U+0007ff		110xxxxx	10xxxxxx
		3 (16 bits):	U+00ffff		U+00ffff		1110xxxx	10xxxxxx	10xxxxxx
		4 (21 bits):	U+010000		U+10ffff		11110xxx	10xxxxxx	10xxxxxx	10xxxxxx
	*/

	return this->bytes() - std::count_if(this->c_str(), this->c_str() + this->bytes(),
		[](char const& c) -> bool
	{
		return (c & 0xc0) == 0x80;
	});
}

} // namespace psi