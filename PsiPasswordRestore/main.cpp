﻿#include "database.hpp"

#include <time.h>

#include <chrono>
#include <iomanip>
#include <iostream>

namespace psi {
namespace detail {

void get_local_time(tm* const time)
{
	auto now = std::chrono::system_clock::to_time_t(std::chrono::system_clock::now());
	localtime_s(time, &now);
}

} // namespace detail

void recover_plaintexts(std::vector<psi::password>& passwords)
{
	passwords[0].resolve(u8"汉字11123");
	passwords[1].resolve("havfun");
}

void repack(int argc, char* argv[])
{
	psi::repacker_t repacker;

	for (int i = 1; i < argc; i++)
		repacker.repack_file(argv[i]);
}

void recover_passwords(int argc, char* argv[])
{
	psi::database db;

	if (db.connect())
	{
		std::vector<psi::password> passwords;
		
		if (db.fetch_passwords(passwords))
		{
			recover_plaintexts(passwords);

			for (psi::password const& password : passwords)
			{
				if (password.resolved())
				{
					printf("Resolved password [id: %I64d => password: %s]\n", password.id, password.plaintext.c_str());
					db.update_password(password);
				}
			}
		}
	}
}

} // namespace psi

#include "repacker.hpp"

int main(int argc, char* argv[])
{
	if (argc < 2)
	{
		std::cout << "Please supply one of the following parameters:" << std::endl;
		std::cout << "-repack <password lists>\tRepack the password list into desired format" << std::endl;
		std::cout << "-recover <password lists>\tRecover passwords from the database" << std::endl;
		return 1;
	}
	else
	{
		tm local_time;
		psi::detail::get_local_time(&local_time);

		std::cout << "Initiated execution of password recovery at " << std::put_time(&local_time, "%T on %b %d, %Y") << "." << std::endl;

		auto start = std::chrono::high_resolution_clock::now();

		if (strcmp(argv[0], "-repack") == 0)
			psi::repack(argc, argv);
		else if (strcmp(argv[0], "-recover") == 0)
			psi::recover_passwords(argc, argv);

		auto end = std::chrono::high_resolution_clock::now();
		auto time_span = std::chrono::duration_cast<std::chrono::duration<double>>(end - start);

		std::cout << "Finished execution of functionality in " << time_span.count() << " seconds" << std::endl;
		std::cin.ignore();
		std::cin.get();
		return 0;
	}
}