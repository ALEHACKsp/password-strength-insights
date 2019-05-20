#pragma once

#include "structures.hpp"

#include <jdbc\mysql_driver.h>
#include <jdbc\mysql_connection.h>

#include <vector>

namespace psi {

class database
{
	std::unique_ptr<sql::Connection> connection;

public:
	database();

	bool connect(
		std::string const& url = "tcp://127.0.0.1:3306", 
		std::string const& schema = "psi",
		std::string const& username = "root", 
		std::string const& password = "Pa$$w0rd");

	bool alive() const;

	bool fetch_passwords(std::vector<psi::password>& passwords) const;
	bool update_password(psi::password const& password) const;
};

} // namespace psi