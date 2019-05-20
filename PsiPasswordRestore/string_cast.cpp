#include "string_cast.hpp"

#include <Windows.h>

namespace psi {
namespace detail {
namespace convert {

int string_convert(codepage const cp, std::string const& input, std::vector<wchar_t>& output)
{
	return MultiByteToWideChar(static_cast<uint32_t>(cp), 0, input.c_str(), static_cast<int>(input.length()),
		(output.empty() ? NULL : output.data()), static_cast<int>(output.size()));
}

int string_convert(codepage const cp, std::wstring const& input, std::vector<char>& output)
{
	return WideCharToMultiByte(static_cast<uint32_t>(cp), 0, input.c_str(), static_cast<int>(input.length()),
		(output.empty() ? NULL : output.data()), static_cast<int>(output.size()), NULL, NULL);
}

} // namespace convert
} // namespace details
} // namespace psi