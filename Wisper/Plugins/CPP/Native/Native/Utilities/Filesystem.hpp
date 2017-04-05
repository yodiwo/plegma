#pragma once
#ifndef YODIWO_UTILITIES_FILESYSTEM_HPP
#define YODIWO_UTILITIES_FILESYSTEM_HPP

#include <set>
#include <regex>
#include <tuple>
#include <regex>
#include <cmath>
#include <random>
#include <vector>
#include <cstdio>
#include <thread>
#include <fstream>
#include <sstream>
#include <iomanip>
#include <cstring>
#include <cstdarg>
#include <iterator>
#include <iostream>
#include <exception>
#include <stdexcept>
#include <algorithm>
#include <functional>
#include <type_traits>
#include <initializer_list>

#include <ftw.h>
#include <stdio.h>
#include <errno.h>
#include <string.h>
#include <dirent.h>
#include <unistd.h>
#include <cxxabi.h>
#include <execinfo.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <sys/sysinfo.h>
#include <linux/unistd.h>
#include <linux/kernel.h>

#include "Enumeration.hpp"

namespace Yodiwo
{
	namespace Utilities
	{
		namespace Filesystem
		{
			//=========================================================

			DEFINE_ENUM_TYPE(
					FileType,
					uint8_t,
					((FILE,0))
					((DIRECTORY,1))
					((OTHER,2))
					((NOT_EXIST,3)));

			//=========================================================

			bool
			Exists(const std::string& filename);

			//=========================================================

			FileType::Enum
			GetFileType(
					const std::string& filename);

			//=========================================================

			struct stat
			FileAttributes(const std::string& filename);

			//=========================================================

			bool
			IsFile(const char* filename);

			//=========================================================

			bool
			IsFile(const std::string& filename);

			//=========================================================

			bool
			IsDirectory(const char* dirname);

			//=========================================================

			bool
			IsDirectory(const std::string& dirname);

			//=========================================================

			bool
			FileExists(const char* filename);

			//=========================================================

			bool
			FileExists(const std::string& filename);

			//=========================================================

			bool
			FilesExist(const std::vector<std::string>& filenames);

			//=========================================================

			bool
			CreateDirectory(const char* dirname);

			//=========================================================

			bool
			CreateDirectory(const std::string& directory);

			//=========================================================

			bool
			DeleteDirectory(const char* directory);

			//=========================================================

			bool
			DeleteDirectory(const std::string& directory);

			//=========================================================

			bool
			Delete(const std::string& filename);

			//=========================================================

			bool
			Delete(const std::vector<std::string>& filenames);

			//=========================================================

			bool
			DeleteFile(const std::string& filename);

			//=========================================================

			bool
			ChangeDirectory(
					const std::string& directory);

			//=========================================================

			std::string
			AbsolutePath(
					const std::string& path);

			//=========================================================

			/*
			 * Return true if the two files|directories refer to the same entity
			 */
			bool
			Same(const std::string& directory1,
				const std::string& directory2);

			//=========================================================

			bool
			StringToFile(
					const std::string& filename,
					const std::string& text);

			//=========================================================

			struct CopyFileResult
			{
				bool success;
				std::string from;
				std::string to;
			};

			//=========================================================

			CopyFileResult
			CopyFile(
					const std::string& fromFilename,
					const std::string& toFilename,
					bool overwrite = true);

			//=========================================================

			std::vector<CopyFileResult>
			CopyFile(
					const std::vector<std::pair<std::string, std::string>>& sourceDestinationTuples,
					bool overwrite = true);

			//=========================================================

			DEFINE_ENUM_TYPE(
					ArchiveOperation,
					uint8_t,
					((Create,0))
					((Extract,1)));

			//=========================================================

			bool
			Archive(
					const std::vector<std::string>& source,
					const std::string& destination,
					ArchiveOperation::Enum operation = ArchiveOperation::Enum::Create);

			//=========================================================

			bool
			Archive(
					const std::string& source,
					const std::string& destination,
					ArchiveOperation::Enum operation = ArchiveOperation::Enum::Create);

			//=========================================================

			bool
			Zip(const std::vector<std::string>& source,
				const std::string& destination,
				ArchiveOperation::Enum operation = ArchiveOperation::Enum::Create);

			//=========================================================

			std::string
			GetCurrentDirectory();

			//=========================================================

			std::string
			FileToString(
					const std::string& filename);

			//=========================================================

			std::string
			GetFilenameFromPath(
					const std::string& filename);

			//=========================================================

			std::string
			GetFilenameFromPathRemoveExtension(
					const std::string& filename);

			//=========================================================

			std::string
			GetFilenameExtension(
					const std::string& filename);

			//=========================================================

			std::string
			GetDirectoryPathOfFile(
					const std::string& filename);

			//=========================================================

			std::vector<std::string>
			OrderFilesBy(
					const std::vector<std::string>& files,
					std::function<bool(const std::string& lhs, const std::string& rhs)> orderFunction = nullptr);

			//=========================================================

			std::vector<std::string>
			FindImageFiles(
					const std::string& dirname);

			//=========================================================

			std::vector<std::string>
			FindFiles(
					const std::string& dirname,
					const std::string& pattern = "");

			//=========================================================

			void FindAndApplyToFiles(
					const std::string& dirname,
					const std::string& pattern,
					std::function<void(const std::string& filename)>  function);

			//=========================================================

			void FindAndApplyToFilesParallel(
					const std::string& dirname,
					const std::string& pattern,
					std::function<void(const std::string& filename)>  function);

			//=========================================================

			void FindAndApplyToImageFiles(
					const std::string& dirname,
					std::function<void(const std::string& filename)>  function);

			//=========================================================

			size_t FindAndCountToFilesSize(
					const std::string& dirname,
					const std::string& pattern = "",
					const bool showFilesSize = false);

			//=========================================================

			size_t CountFiles(
					const std::string& dirname,
					const std::string& pattern = "");

			//=========================================================

			size_t CountImageFiles(
					const std::string& dirname);

			//=========================================================

			size_t GetFreeSize(
					const std::string& path);

			//=========================================================
			/*
			 * Iterate file line by line and execute function at each line
			 */
			void LineByLine(
					const std::string& filename,
					std::function<void(const std::string& line)> function);

			//=========================================================
			/*
			 * Iterate stream line by line and execute function at each line
			 */
			void LineByLine(
				std::ifstream& stream,
				std::function<void(const std::string& line)> function);

			//=========================================================

			std::vector<std::string>
			FindDirectories(
					const std::string& dirname,
					const std::string& pattern = "");

			//=========================================================

			int
			DeleteFolderCallback(
					const char *fpath,
					const struct stat *sb,
					int typeflag,
					struct FTW *ftwbuf);

			//=========================================================

			size_t
			GetFileSize(
					const std::string& filename);

			//=========================================================
			/*
			 * Returns a filename in /tmp that
			 * certainly does not exists and can be used for scratch pad
			 */
			std::string
			GetTmpFilename(
					const std::string& prefix = "./",
					const std::string& postfix = "",
					const uint8_t length = 16,
					const bool includeTimeStamp = false);

			//=========================================================

			bool
			ShuffleFile(const std::string& inFile,
					const std::string& outFile);

			//=========================================================

			bool
			ShuffleFile(const std::string& file);

			//=========================================================

			bool
			EndsWith(
					const std::string& file,
					const std::string& pattern);

			//=========================================================

			bool
			IsCSV(const std::string& file);

			//=========================================================

			bool
			IsJson(const std::string& file);

			//=========================================================
		};
	};
}

#endif
