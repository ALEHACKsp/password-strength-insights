#include "database.hpp"

#include <jdbc\cppconn\prepared_statement.h>
#include <jdbc\cppconn\resultset.h>

namespace psi {
namespace detail {

sql::Driver* driver()
{
	static sql::Driver* driver = nullptr;

	if (driver == nullptr)
		driver = get_driver_instance();

	return driver;
}

} // namespace detail

database::database() :
	connection(nullptr)
{

}

bool database::connect(std::string const& url, std::string const& schema, std::string const& username, std::string const& password)
{
	try
	{
		this->connection = std::unique_ptr<sql::Connection>(detail::driver()->connect(url, username, password));
		this->connection->setSchema(schema);

		return this->alive();
	}
	catch (sql::SQLException const& e)
	{
		std::cout << "SQL Exception: " << e.what() << std::endl;
		return false;
	}
}

bool database::alive() const
{
	return (this->connection->isValid() && !this->connection->isClosed());
}

bool database::fetch_passwords(std::vector<psi::password_t>& passwords) const
{
	try
	{
		if (!this->alive())
			return false;
		else
		{
			std::unique_ptr<sql::PreparedStatement> statement(this->connection->prepareStatement("SELECT * FROM `Passwords` WHERE `plaintext` IS NULL;"));
			std::unique_ptr<sql::ResultSet> result_set(statement->executeQuery());

			while (result_set->next())
			{
				std::unique_ptr<std::istream> nt_blob(result_set->getBlob("nt"));
				std::unique_ptr<std::istream> lm_blob(result_set->getBlob("lm"));

				uint64_t id = result_set->getUInt64("id");

				std::vector<uint8_t> nt;
				nt.assign(std::istreambuf_iterator<char>(*nt_blob), std::istreambuf_iterator<char>());

				std::vector<uint8_t> lm;
				lm.assign(std::istreambuf_iterator<char>(*lm_blob), std::istreambuf_iterator<char>());

				passwords.push_back(psi::password_t(id, nt, lm));
			}

			return true;
		}
	}
	catch (sql::SQLException const& e)
	{
		std::cout << "SQL Exception: " << e.what() << std::endl;
		return false;
	}
}

bool database::update_password(psi::password_t const& password) const
{
	try
	{
		if (!this->alive())
			return false;
		else
		{
			std::unique_ptr<sql::PreparedStatement> statement(this->connection->prepareStatement("UPDATE `Passwords` SET `plaintext` = ? WHERE `id` = ?"));

			statement->setString(1, password.plaintext.c_str());
			statement->setUInt64(2, password.id);

			return (statement->executeUpdate() != 0);
		}
	}
	catch (sql::SQLException const& e)
	{
		std::cout << "SQL Exception: " << e.what() << std::endl;
		return false;
	}
}

} // namespace psi