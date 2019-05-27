#include "hash_list.hpp"

#include <Windows.h>

namespace psi {

hash_list_t::hash_list_t() :
	handle(INVALID_HANDLE_VALUE),
	bytes(0),
	count(0)
{

}

hash_list_t::~hash_list_t()
{
	close();
}

void hash_list_t::open(std::string const& filename)
{
	close();

	if ((this->handle = CreateFileA(filename.c_str(), GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_FLAG_SEQUENTIAL_SCAN, NULL)) == INVALID_HANDLE_VALUE)
		throw std::runtime_error("cannot open file");
	else
	{
		LARGE_INTEGER file_size;
		memset(&file_size, 0, sizeof(LARGE_INTEGER));

		if (!GetFileSizeEx(this->handle, &file_size))
			throw std::runtime_error("cannot query file size");
		else if (read(&this->count, sizeof(uint64_t)) != sizeof(uint64_t))
			throw std::runtime_error("cannot read entire element count object");
		else
			this->bytes = (static_cast<uint64_t>(file_size.QuadPart) - sizeof(uint64_t));
	}
}

void hash_list_t::close()
{
	if (this->handle != INVALID_HANDLE_VALUE)
	{
		CloseHandle(this->handle);
		this->handle = INVALID_HANDLE_VALUE;
	}

	this->bytes = 0;
	this->count = 0;
}

void hash_list_t::reset() const
{
	if (this->handle == INVALID_HANDLE_VALUE)
		throw std::runtime_error("cannot reset uninitialized file");
	else if (SetFilePointer(this->handle, sizeof(uint64_t), NULL, FILE_BEGIN) == INVALID_SET_FILE_POINTER && GetLastError() != NO_ERROR)
		throw std::runtime_error("cannot reset file pointer");
}

uint32_t hash_list_t::read(void* const buffer, std::size_t length) const
{
	if (this->handle == INVALID_HANDLE_VALUE)
		throw std::runtime_error("cannot read from uninitialized file");
	else
	{
		DWORD bytes_read = 0;

		if (!ReadFile(this->handle, buffer, static_cast<DWORD>(length), &bytes_read, NULL))
			throw std::runtime_error("cannot read file data");
		else
			return static_cast<uint32_t>(bytes_read);
	}
}

uint64_t hash_list_t::size() const
{
	return this->bytes;
}

uint64_t hash_list_t::elements() const
{
	return this->count;
}

} // namespace psi