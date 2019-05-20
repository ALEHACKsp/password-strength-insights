#pragma once

#include "hash_structure.hpp"
#include "string_structure.hpp"

#include <filesystem>
#include <fstream>
#include <iostream>
#include <iterator>
#include <string>

namespace psi {

template <class HashFormat, class ParserFormat>
class parser_t
{
	using password_pair_t = std::pair<string_t, hash_t<HashFormat::size>>;

	class text_line : public std::string
	{
		friend std::istream& operator>>(std::istream& is, text_line& line)
		{
			return std::getline(is, line);
		}
	};

public:
	parser_t() :
		hasher(HashFormat()),
		parser(ParserFormat())
	{

	}

	template <typename... Delims>
	parser_t(Delims&&... delims) :
		hasher(HashFormat()),
		parser(ParserFormat({ std::forward<Delims>(delims)... }))
	{

	}

	std::vector<password_pair_t> read_file(std::string const& path)
	{
		std::vector<password_pair_t> results;
		this->iterate_file(path, results);
		return results;
	}

	std::vector<password_pair_t> read_directory(std::string const& path)
	{
		std::vector<password_pair_t> results;
		this->iterate_directory(path, results);
		return results;
	}

private:
	void iterate_directory(std::string const& path, std::vector<password_pair_t>& results)
	{
		std::experimental::filesystem::directory_iterator directory(path);

		for (std::experimental::filesystem::directory_iterator::value_type const& entry : directory)
		{
			switch (entry.status().type())
			{
			case std::experimental::filesystem::file_type::directory:
				this->iterate_directory(entry.path().string(), results);
				break;

			case std::experimental::filesystem::file_type::regular:
				this->iterate_file(entry.path().string(), results);
				break;

			default:
				throw std::invalid_argument("invalid file type");
			}
		}
	}

	void iterate_file(std::string const& path, std::vector<password_pair_t>& results)
	{
		std::cout << "Parsing \"" << path << "\"...";

		std::ifstream file(path);

		std::istream_iterator<text_line> begin(file);
		std::istream_iterator<text_line> end;

		for (std::istream_iterator<text_line> iterator = begin; iterator != end; iterator++)
		{
			std::string plaintext = this->parser(*iterator);

			if (!plaintext.empty() && std::find(plaintext.begin(), plaintext.end(), '\0') == plaintext.end())
				results.push_back(password_pair_t(string_t(plaintext), hash_t<HashFormat::size>(this->hasher(plaintext.data(), plaintext.length()))));
		}

		std::cout << "Finished parsing." << std::endl;
	}

private:
	HashFormat hasher;
	ParserFormat parser;
};

} // namespace psi