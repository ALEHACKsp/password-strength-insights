#include "parser_types.hpp"

#include <iostream>

namespace psi {
namespace detail {

delimited_parser::delimited_parser(char delimiter) :
	delimited_parser({ delimiter })
{

}

delimited_parser::delimited_parser(std::initializer_list<char> delimiters) :
	delimiters(delimiters)
{

}

} // namespace detail

std::string password_format::operator()(std::string const& input) const
{
	return input;
}

std::string username_password_format::operator()(std::string const& input) const
{
	std::size_t x;

	for (char const& delimiter : this->delimiters)
	{
		if ((x = input.find(delimiter)) != std::string::npos)
			return input.substr(x + 1);				// username:password
	}

	return std::string();
}

std::string email_password_format::operator()(std::string const& input) const
{
	std::size_t x, y;

	if ((x = input.find('@')) != std::string::npos)
	{
		for (char const& delimiter : this->delimiters)
		{
			if ((y = input.find(delimiter)) != std::string::npos)
			{
				if (y < x)
					return input.substr(0, y);		// password:email
				else if ((y + 1) < input.size())
					return input.substr(y + 1);		// email:password
			}
		}
	}

	return std::string();
}

} // namespace psi