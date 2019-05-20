#pragma once

#include <array>
#include <string>
#include <vector>

namespace psi {
namespace detail {

struct base_parser
{
	virtual std::string operator()(std::string const& input) const = 0;
};

struct delimited_parser : public base_parser
{
	delimited_parser(char delimiter);
	delimited_parser(std::initializer_list<char> delimiters);

protected:
	std::vector<char> delimiters;
};

} // namespace detail

  // Parses the format 'password'
struct password_format : public detail::base_parser
{
	std::string operator()(std::string const& input) const override;
};

// Parses the format 'username-password', where '-' is the delimiter
struct username_password_format : public detail::delimited_parser
{
	using delimited_parser::delimited_parser;

	std::string operator()(std::string const& input) const override;
};

// Parses the format 'email-password', where '-' is the delimiter
struct email_password_format : public detail::delimited_parser
{
	using delimited_parser::delimited_parser;

	std::string operator()(std::string const& input) const override;
};

} // namespace psi