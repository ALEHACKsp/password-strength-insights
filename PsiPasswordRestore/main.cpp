#include "database.hpp"

#include "repacker.hpp"
#include "resolver.hpp"

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

void repack(int argc, char* argv[])
{
	psi::repacker_t repacker;

	for (int i = 2; i < argc; i++)
		repacker.repack_file(argv[i]);
}

void recover_passwords(int argc, char* argv[])
{
	std::vector<std::pair<hash_list_t, plaintext_list_t>> lookups;

	for (int i = 2; i < argc; i++)
	{
		lookups.push_back(std::make_pair(hash_list_t(), plaintext_list_t()));
		lookups.back().first.open(argv[i] + std::string(".hl"));
		lookups.back().second.open(argv[i] + std::string(".pl"));
	}

	if (!lookups.empty())
	{
		psi::database db;

		if (db.connect())
		{
			std::vector<psi::password_t> passwords;

			if (db.fetch_passwords(passwords))
			{
				resolver_t resolver;
				resolver.test(lookups, passwords);

				for (psi::password_t const& password : passwords)
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
}

} // namespace psi

int main(int argc, char* argv[])
{
	if (argc < 3)
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

		if (strcmp(argv[1], "-repack") == 0)
			psi::repack(argc, argv);
		else if (strcmp(argv[1], "-recover") == 0)
			psi::recover_passwords(argc, argv);

		auto end = std::chrono::high_resolution_clock::now();
		auto time_span = std::chrono::duration_cast<std::chrono::duration<double>>(end - start);

		std::cout << "Finished execution of functionality in " << time_span.count() << " seconds" << std::endl;
		return 0;
	}
}