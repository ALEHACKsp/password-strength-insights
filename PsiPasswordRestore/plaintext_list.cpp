#include "plaintext_list.hpp"

#include <Windows.h>
#include <iostream>

namespace psi {

plaintext_list_t::plaintext_list_t() : 
	handle(INVALID_HANDLE_VALUE),
	bytes(0)
{

}

plaintext_list_t::~plaintext_list_t()
{
	close();
}

void plaintext_list_t::open(std::string const& path)
{
	close();

	if ((this->handle = CreateFileA(path.c_str(), GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_FLAG_SEQUENTIAL_SCAN, NULL)) == INVALID_HANDLE_VALUE)
		throw std::runtime_error("cannot open file");
	else
	{
		LARGE_INTEGER file_size;
		memset(&file_size, 0, sizeof(LARGE_INTEGER));

		if (!GetFileSizeEx(this->handle, &file_size))
			throw std::runtime_error("cannot query file size");
		else
			this->bytes = (static_cast<uint64_t>(file_size.QuadPart) - sizeof(uint64_t));
	}
}

void plaintext_list_t::close()
{
	if (this->handle != INVALID_HANDLE_VALUE)
	{
		CloseHandle(this->handle);
		this->handle = INVALID_HANDLE_VALUE;
	}

	this->bytes = 0;
}

std::string plaintext_list_t::read(uint64_t index) const
{
	point(index * sizeof(uint64_t));

	uint64_t offset = 0;
	uint32_t b = this->read(&offset, sizeof(uint64_t));

	if (b != sizeof(uint64_t))
		throw std::runtime_error("cannot read file offset");
	else
	{
		point(offset);

		char buf[64];
		b = this->read(buf, 64);

		return std::string(buf);
	}
}

uint32_t plaintext_list_t::read(void* const buffer, std::size_t length) const
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

void plaintext_list_t::point(uint64_t offset) const
{
	LONG dwHigh = offset >> 32;

	if (this->handle == INVALID_HANDLE_VALUE)
		throw std::runtime_error("cannot reset uninitialized file");
	else if (SetFilePointer(this->handle, offset & 0xffffffff, &dwHigh, FILE_BEGIN) == INVALID_SET_FILE_POINTER && GetLastError() != NO_ERROR)
		throw std::runtime_error("cannot reset file pointer");
}

} // namespace psi