#include "Macros.hpp"
#include "Time.hpp"
#include "String.hpp"
#include "Constants.hpp"
#include "Filesystem.hpp"

using namespace Yodiwo;
using namespace Yodiwo::Utilities;

//=========================================================================

size_t
Filesystem::GetFreeSize(
		const std::string& path)
{
	return size_t(0);
}

//=========================================================================

/*
 * Try to open a file/directory to verify it is there
 */
bool
Filesystem::Exists(
		const std::string& filename)
{
	if (filename.empty() == true)
	{
		return true;
	}

	try
	{
		auto file = std::fopen(filename.c_str(), "r");

		if (file == NULL)
		{
			return false;
		}

		std::fclose(file);
	}
	catch(const std::exception& exp)
	{
		return false;
	}

	return true;
}

//=========================================================================

Filesystem::FileType::Enum
Filesystem::GetFileType(
		const std::string& filename)
{
	if (Exists(filename) == false)
	{
		return FileType::NOT_EXIST;
	}

    struct stat path_stat = {0};
    stat(filename.c_str(), &path_stat);

    if (S_ISREG(path_stat.st_mode) == 1)
    {
    	return FileType::FILE;
    }

    if (S_ISDIR(path_stat.st_mode) == 1)
    {
       	return FileType::DIRECTORY;
    }

    return FileType::OTHER;
}

//=========================================================================

size_t
Filesystem::GetFileSize(
		const std::string& filename)
{
	auto filename_c_str = filename.c_str();

	if (Filesystem::FileExists(filename) == false)
	{
		GENERIC_EXCEPTION("File [%s] does not exists", filename_c_str);
	}

	if (Filesystem::IsFile(filename) == false)
	{
		GENERIC_EXCEPTION("[%s] is not a file", filename_c_str);
	}

	try
	{
	    std::streampos fsize = 0;
	    std::ifstream file(filename, std::ios::binary);

	    fsize = file.tellg();
	    file.seekg(0, std::ios::end);
	    fsize = file.tellg() - fsize;
	    file.close();

	    return static_cast<size_t>(fsize);
	}
	catch(const std::exception& exp)
	{
		ERROR_MESSAGE("Exception: %s", exp.what());
		GENERIC_EXCEPTION("Failed to get size of [%s]", filename_c_str);
	}

	return 0;
}

//=========================================================================

struct stat
Filesystem::FileAttributes(const std::string& filename)
{
	struct stat buffer = {0};

	if (stat(filename.c_str(), &buffer) != 0)
	{
		GENERIC_EXCEPTION("Not a valid file");
	}

	return buffer;
}

//=========================================================================

std::string
Filesystem::GetTmpFilename(
		const std::string& prefix,
		const std::string& postfix,
		const uint8_t length,
		const bool includeTimeStamp)
{
	int maxRetries = 999;

	while (maxRetries-- > 0)
	{
		std::stringstream ss;
		ss << prefix << "/" << String::Random(length);

		if (includeTimeStamp == true)
		{
			ss << Time::DateStamp();
		}

		ss << postfix;

		auto result = ss.str();

		if (Filesystem::FileExists(result) == false)
		{
			return result;
		}
	}

	GENERIC_EXCEPTION("Failed to find random tmp file");
}

//=========================================================================

bool
Filesystem::FileExists(
		const char* filename)
{
	struct stat buffer = {0};
	return (stat(filename, &buffer) == 0);
}

//=========================================================================

bool
Filesystem::FileExists(
		const std::string& filename)
{
	return FileExists(filename.c_str());
}

//=========================================================================

bool
Filesystem::FilesExist(
		const std::vector<std::string>& filenames)
{
	bool result = true;

	for (const auto& filename : filenames)
	{
		result &= FileExists(filename.c_str());
	}

	return result;
}

//=========================================================================

bool
Filesystem::IsDirectory(
		const std::string& dirname)
{
	return IsDirectory(dirname.c_str());
}

//=========================================================================

bool
Filesystem::IsDirectory(
		const char* dirname)
{
	if (dirname == NULL || strlen(dirname) <= 0)
	{
		GENERIC_EXCEPTION("Directory cannot be NULL or empty");
	}

	struct stat sb = {0};

	if (stat(dirname, &sb) == 0)
	{
		if (S_ISDIR(sb.st_mode))
		{
			return true;
		}
	}

	return false;
}

//=========================================================================

bool
Filesystem::IsFile(
		const std::string& filename)
{
	return IsFile(filename.c_str());
}

//=========================================================================

bool
Filesystem::IsFile(
		const char* filename)
{
	if (filename == NULL)
	{
		GENERIC_EXCEPTION("File cannot be NULL or empty");
	}

	struct stat sb = {0};

	if (stat(filename, &sb) == 0)
	{
		if (S_ISREG(sb.st_mode))
		{
			return true;
		}
	}

	return false;
}

//=========================================================================

bool Filesystem::CreateDirectory(
		const std::string& dirname)
{
	return CreateDirectory(dirname.c_str());
}

//=========================================================================

bool
Filesystem::CreateDirectory(
		const char* dirname)
{
	if (dirname == NULL || std::strlen(dirname) <= 0)
	{
		GENERIC_EXCEPTION("Directory cannot be NULL or empty");
	}

	if (IsFile(dirname) == true)
	{
		ERROR_MESSAGE("Couldn't create directory [%s] because it is an existing file", dirname);
		return false;
	}

	if (IsDirectory(dirname) == true)
	{
		return true;
	}

	char* tmpDirname = (char*)dirname;

	while (tmpDirname != NULL && std::strlen(tmpDirname) > 0)
	{
		char path[PATH_MAX] = {0};
		char* dashChar = std::strchr(tmpDirname, '/');

		if (dashChar != NULL)
		{
			std::strncpy(path, dirname, dashChar - dirname);
			tmpDirname = dashChar + 1;
		}
		else
		{
			std::strncpy(path, dirname, PATH_MAX);
			tmpDirname = NULL;
		}

		if (std::strlen(path) == 0 && dashChar != NULL)
		{
			// special case for root (/)
			continue;
		}
		else if (IsFile(path) == false && IsDirectory(path) == false)
		{
			int status = mkdir(path, S_IRWXU | S_IRWXG | S_IROTH | S_IXOTH);

			if (status != 0)
			{
				ERROR_MESSAGE("Couldn't create directory [%s]", path);
				return false;
			}
		}
	}

	return true;
}

//=========================================================================

/*
 * Returns the directory of the filename,
 * string opearation, not a filesystem operation
 */
std::string
Filesystem::GetDirectoryPathOfFile(
		const std::string& filename)
{
	size_t found = 0;
	found = filename.find_last_of("/\\");

	if (found == filename.npos)
	{
		return std::string("./");
	}

	return filename.substr(0,found);
}

//=========================================================================

std::string
Filesystem::AbsolutePath(
		const std::string& directory)
{
	char path[PATH_MAX] = {0};

	if (realpath(directory.c_str(), path) == NULL)
	{
		GENERIC_EXCEPTION("Failed to find realpath of [%s]", directory.c_str());
	}

	return std::string(path);
}

//=========================================================================

/*
 * Return absolute path to current location
 */
std::string
Filesystem::GetCurrentDirectory()
{
	char path[PATH_MAX] = {0};

	if (getcwd(path, PATH_MAX) == NULL)
	{
		ERROR_MESSAGE("Could not locate current directory");
	}

	return std::string(path);
}

//=========================================================================

bool
Filesystem::ChangeDirectory(
		const std::string& directory)
{
	if (directory.empty() == true)
	{
		return false;
	}

	if (IsDirectory(directory) == false)
	{
		return false;
	}

	int status = chdir(directory.c_str());

	if (status == 0)
	{
		return true;
	}

	return false;
}

//=========================================================================

std::vector<std::string>
Filesystem::FindImageFiles(
		const std::string& dirname)
{
	return FindFiles(dirname, IMAGE_FILES_REGEX);
}

//=========================================================================

bool
Filesystem::StringToFile(
		const std::string& filename,
		const std::string& text)
{
	try
	{
		std::ofstream ofs;
		// any operation that sets the error flag on file throws exception
		ofs.exceptions(std::ios::failbit);
		ofs.open(filename);
		ofs << text;
		ofs.close();
	}
	catch(const std::exception& exp)
	{
		ERROR_MESSAGE("Exception: %s", exp.what());
		ERROR_MESSAGE("Failed to write to file [%s]", filename.c_str());
		return false;
	}

	return true;
}

//=========================================================================

std::string
Filesystem::FileToString(
		const std::string& filename)
{
	try
	{
		std::ifstream ifs;
		// any operation that sets the error flag on file throws exception
		ifs.exceptions(std::ios::failbit);
		//open the input file
		ifs.open(filename);
		std::stringstream ss;
		//read the file
		ss << ifs.rdbuf();
		//str holds the content of the file
		return ss.str();
	}
	catch(const std::exception& exp)
	{
		ERROR_MESSAGE("Exception: %s", exp.what());
		GENERIC_EXCEPTION("Failed to read file %s to string", filename.c_str());
	}
}

//=========================================================================

std::vector<std::string>
Filesystem::OrderFilesBy(
		const std::vector<std::string>& files,
		std::function<bool(const std::string& lhs, const std::string& rhs)> orderFunction)
{
	if (orderFunction == nullptr)
	{
		return files;
	}

	std::vector<std::string> results(files);

	std::sort(results.begin(), results.end(), orderFunction);

	return results;
}

//=========================================================================

std::vector<std::string>
Filesystem::FindFiles(
		const std::string& dirname,
		const std::string& pattern)
{
	if (dirname.empty() == true)
	{
		GENERIC_EXCEPTION("Directory cannot be NULL or empty");
	}

	std::vector<std::string> files;

	auto fileSizeFunction =
		[&files](const std::string& filename) mutable
		{
			try
			{
				if (IsFile(filename) == true)
				{
					files.push_back(filename);
				}
			}
			catch(const std::exception& exp)
			{
				ERROR_MESSAGE("Failed to get file size of [%s]. Exception: %s",
						filename.c_str(), exp.what());
			}
		};

	Filesystem::FindAndApplyToFiles(dirname, pattern, fileSizeFunction);

	return files;
}

//=========================================================================

size_t
Filesystem::FindAndCountToFilesSize(
		const std::string& dirname,
		const std::string& pattern,
		const bool verbose)
{
	size_t totalSize = 0;

	auto fileSizeFunction =
		[&totalSize, verbose](const std::string& filename) mutable
		{
			try
			{
				if (Filesystem::IsFile(filename) == true)
				{
					auto size = Filesystem::GetFileSize(filename);
					totalSize += size;

					if (verbose == true)
					{
						std::cout << filename << " " << size << " bytes" << std::endl;
					}
				}
			}
			catch(const std::exception& exp)
			{
				ERROR_MESSAGE("Failed to get file size of [%s]. Exception: %s",
						filename.c_str(), exp.what());
			}
		};

	Filesystem::FindAndApplyToFiles("./", "", fileSizeFunction);

	return totalSize;
}

//=========================================================================

size_t
Filesystem::CountFiles(
		const std::string& dirname,
		const std::string& pattern)
{
	size_t count = 0;

	auto fileSizeFunction =
		[&count](const std::string& filename) mutable
		{
			try
			{
				if (Filesystem::IsFile(filename) == true)
				{
					count++;
				}
			}
			catch(const std::exception& exp)
			{
				ERROR_MESSAGE("Failed to get count file [%s]. Exception: %s",
						filename.c_str(), exp.what());
			}
		};

	Filesystem::FindAndApplyToFiles(dirname, "", fileSizeFunction);

	return count;
}

//=========================================================================

size_t
Filesystem::CountImageFiles(
		const std::string& dirname)
{
	return CountFiles(dirname, IMAGE_FILES_REGEX);
}

//=========================================================================

void
Filesystem::FindAndApplyToImageFiles(
		const std::string& dirname,
		std::function<void(const std::string& filename)>  function)
{
	return FindAndApplyToFiles(dirname, IMAGE_FILES_REGEX, function);
}

//=========================================================================

void
Filesystem::FindAndApplyToFiles(
		const std::string& dirname,
		const std::string& pattern,
		std::function<void(const std::string& filename)>  function)
{
	if (dirname.empty() == true)
	{
		GENERIC_EXCEPTION("Directory cannot be NULL or empty");
	}

    DIR *dir = NULL;
    struct dirent *entry = NULL;
    std::regex filenameMatcher;

    if (pattern.empty() == false)
    {
    	filenameMatcher = std::regex(pattern);
    }

    if ((dir = opendir(dirname.c_str())) == NULL)
    {
    	return;
    }

    if ((entry = readdir(dir)) == NULL)
    {
    	return;
    }

    std::string dirnamePlusSlash = dirname + "/";

    do
    {
        if (entry->d_type == DT_DIR)
        {
            if (strcmp(entry->d_name, ".") == 0 ||
            	strcmp(entry->d_name, "..") == 0)
            {
                continue;
            }

            FindAndApplyToFiles(
            		dirnamePlusSlash + entry->d_name,
            		pattern,
            		function);
        }
        else
        {
        	if (pattern.empty() ||
        			std::regex_search(entry->d_name, filenameMatcher))
        	{
        		if (function != nullptr)
        		{
        			function(dirnamePlusSlash + entry->d_name);
        		}
        	}
        }
    }
    while ((entry = readdir(dir)) != NULL);

    closedir(dir);
}

//=========================================================================

void
Filesystem::LineByLine(
	const std::string& filename,
	std::function<void(const std::string& line)> function)
{
	std::ifstream stream(filename);

	if (!stream)
	{
		GENERIC_EXCEPTION("File [%s] is not valid", filename.c_str());
	}

	LineByLine(stream, function);
}

//=========================================================================

void
Filesystem::LineByLine(
	std::ifstream& stream,
	std::function<void(const std::string& line)> function)
{
	if (!stream)
	{
		GENERIC_EXCEPTION("Stream is not working");
	}

	try
	{
		while (stream.eof() == false)
		{
			std::string line;
			std::getline(stream, line);

			if (line.empty() == true)
			{
				continue;
			}

			function(line);
		}
	}
	catch (const std::exception& exp)
	{
		ERROR_MESSAGE("Exception : %s", exp.what());
		GENERIC_EXCEPTION("Failed to read stream");
	}
}

//=========================================================================

std::vector<std::string>
Filesystem::FindDirectories(
		const std::string& dirname,
		const std::string& pattern)
{
	if (dirname.empty() == true)
	{
		GENERIC_EXCEPTION("Directory cannot be NULL or empty");
	}

    DIR *dir = NULL;
    struct dirent *entry = NULL;
    std::regex filenameMatcher;
    std::vector<std::string> directories;

    if (pattern.empty() == false)
    {
    	filenameMatcher = std::regex(pattern);
    }

    if ((dir = opendir(dirname.c_str())) == NULL)
    {
    	return directories;
    }

    if ((entry = readdir(dir)) == NULL)
    {
    	return directories;
    }

    do
    {
        if (entry->d_type == DT_DIR)
        {
            if (strcmp(entry->d_name, ".") == 0 ||
            	strcmp(entry->d_name, "..") == 0)
            {
                continue;
            }

        	std::string newDirname = dirname + "/" + entry->d_name;

        	if (pattern.empty() ||
        			std::regex_search(entry->d_name, filenameMatcher))
        	{
        		directories.emplace_back(newDirname);
        	}

            auto newDirnameFiles = FindDirectories(newDirname, pattern);

            directories.insert(
            		directories.end(),
            		std::make_move_iterator(newDirnameFiles.begin()),
					std::make_move_iterator(newDirnameFiles.end()));
        }
    }
    while ((entry = readdir(dir)) != NULL);

    closedir(dir);

	return directories;
}

//=========================================================================

bool
Filesystem::DeleteFile(
		const std::string& filename)
{
	if (IsFile(filename) == true)
	{
		int status = std::remove(filename.c_str());

		if (status == 0)
		{
			return true;
		}
	}
	return false;
}

//=========================================================================

/*
 * Delete directory, by also deleting everything inside
 */
bool
Filesystem::DeleteDirectory(
		const std::string& dirname)
{
	return DeleteDirectory(dirname.c_str());
}

//=========================================================================

bool
Filesystem::Delete(
		const std::string& filename)
{
	if (IsFile(filename) == true)
	{
		return DeleteFile(filename);
	}

	if (IsDirectory(filename) == true)
	{
		FindAndApplyToFiles(filename,"", [](const std::string& filename){DeleteFile(filename);});
		return DeleteDirectory(filename);
	}

	return false;
}

//=========================================================================

bool
Filesystem::Delete(
		const std::vector<std::string>& filenames)
{
	bool result = true;

	for (const auto& filename : filenames)
	{
		auto tmpResult = Delete(filename);

		if (tmpResult == false)
		{
			ERROR_MESSAGE("Failed to delete [%s]", filename.c_str());
		}

		result &= tmpResult;
	}

	return result;
}

//=========================================================================

int
Filesystem::DeleteFolderCallback(
		const char *fpath,
		const struct stat *sb,
		int typeflag,
		struct FTW *ftwbuf)
{
    int rv = remove(fpath);

    if (rv != 0)
    {
        perror(fpath);
    }

    return rv;
}

//=========================================================================

/*
 * Delete directory, by also deleting everything inside
 */
bool
Filesystem::DeleteDirectory(
		const char* dirname)
{
	if (dirname == NULL  || strlen(dirname) == 0)
	{
		GENERIC_EXCEPTION("Directory cannot be NULL or empty");
	}

	if (IsDirectory(dirname) == true)
	{
		if (nftw(dirname, DeleteFolderCallback, 64, FTW_DEPTH | FTW_PHYS) == 0)
		{
			return true;
		}
	}

	return false;
}

//=========================================================================

std::vector<Filesystem::CopyFileResult>
Filesystem::CopyFile(
		const std::vector<std::pair<std::string, std::string>>& files,
		bool overwrite)
{
	std::vector<CopyFileResult> results;

	for (const auto& sourceDestinationPair : files)
	{
		auto source = sourceDestinationPair.first;
		auto destination = sourceDestinationPair.second;
		auto tmpResult = CopyFile(source, destination, overwrite);

		if (tmpResult.success == false)
		{
			ERROR_MESSAGE("Failed to copy [%s] to [%s]",
					source.c_str(), destination.c_str());
		}

		results.push_back(tmpResult);
	}

	return results;
}

//=========================================================================

// TODO add more options,
Filesystem::CopyFileResult
Filesystem::CopyFile(
		const std::string& fromFilename,
		const std::string& toFilename,
		bool overwrite)
{
	if (fromFilename.empty() || toFilename.empty())
	{
		GENERIC_EXCEPTION("Invalid arguments");
	}

	if (IsFile(fromFilename) == false)
	{
		return {false, fromFilename, toFilename};
	}

	std::string tmpToFilename(toFilename);

	if (IsDirectory(tmpToFilename) == true)
	{
		tmpToFilename += "/" + fromFilename;
	}

	try
	{
		if (overwrite == true && Exists(toFilename) == true)
		{
			Delete(toFilename);
		}

		std::ifstream src(fromFilename, std::ios::binary);
		std::ofstream dst(tmpToFilename, std::ios::binary);
		dst << src.rdbuf();
	}
	catch(const std::exception& exp)
	{
		ERROR_MESSAGE("Exception: %s", exp.what());
		ERROR_MESSAGE("Failed to copy file [%s] to [%s]",
				fromFilename.c_str(), toFilename.c_str());
		return {false, fromFilename, tmpToFilename};
	}

	return {true, fromFilename, tmpToFilename};
}

//=========================================================================

bool
Filesystem::Archive(
		const std::string& source,
		const std::string& destination,
		ArchiveOperation::Enum operation)
{
	if (IsDirectory(source) == true &&
		operation == ArchiveOperation::Enum::Create)
	{
		const auto root = GetCurrentDirectory();
		const auto absSource = Filesystem::AbsolutePath(source);
		const auto absDestination =
				Filesystem::AbsolutePath(Filesystem::GetDirectoryPathOfFile(destination)) + "/" +
				Filesystem::GetFilenameFromPath(destination);

		if (ChangeDirectory(absSource) == false)
		{
			ERROR_MESSAGE("Failed to change directory [%s]", absSource.c_str());
		}

		auto result = Archive(std::vector<std::string>{"./"}, absDestination, operation);

		if (ChangeDirectory(root) == false)
		{
			ERROR_MESSAGE("Failed to change directory [%s]", root.c_str());
		}

		return result;
	}

	return Archive(std::vector<std::string>{source}, destination, operation);
}

//=========================================================================

bool
Filesystem::Archive(
		const std::vector<std::string>& source,
		const std::string& destination,
		ArchiveOperation::Enum operation)
{
	// TODO to be replaced with libtar
	// TODO add verbose option
	if (source.size() <= 0 || destination.empty() == true)
	{
		return false;
	}

	std::stringstream ss;

	if (operation == ArchiveOperation::Enum::Create)
	{
		ss << "tar -cf ";

		ss << destination;

		for (auto& sourceFile : source)
		{
			ss << " " << sourceFile;
		}
	}
	else if (operation == ArchiveOperation::Enum::Extract)
	{
		ss << "tar -xf ";

		if (FileExists(destination) == true && IsFile(destination) == true)
		{
			return false;
		}

		if (CreateDirectory(destination) == false)
		{
			return false;
		}

		for (auto& sourceFile : source)
		{
			ss << " " << sourceFile;
		}

		ss << " -C " << destination;
	}
	else
	{
		return false;
	}

	auto result = std::system(ss.str().c_str());

	if (result != SYSTEM_SUCCESS)
	{
		INFO_MESSAGE("Failed to tar [%s], Trying zip fallback", destination.c_str());
		return Zip(source,destination, operation);
	}

	return true;
}

//=========================================================================

bool
Filesystem::Zip(
		const std::vector<std::string>& source,
		const std::string& destination,
		ArchiveOperation::Enum operation)
{
	// TODO to be replaced with libtar
	if (source.size() <= 0 || destination.empty() == true)
	{
		return false;
	}

	std::stringstream ss;

	if (operation == ArchiveOperation::Enum::Create)
	{
		ss << "zip ";

		ss << destination;

		for (auto& sourceFile : source)
		{
			ss << " " << sourceFile;
		}
	}
	else if (operation == ArchiveOperation::Enum::Extract)
	{
		ss << "unzip ";

		if (FileExists(destination) == true && IsFile(destination) == true)
		{
			return false;
		}

		if (CreateDirectory(destination) == false)
		{
			return false;
		}

		for (auto& sourceFile : source)
		{
			ss << " " << sourceFile;
		}

		ss << " -d " << destination;
	}
	else
	{
		return false;
	}

	auto result = std::system(ss.str().c_str());

	/*
	 *  0 normal no errors or warnings detected.
	 *
	 *  1 one or more warning errors were encountered, but processing completed successfully anyway.
	 *  This includes zipfiles where one or more files was skipped due to unsupported compression
	 *  method or encryption with an unknown password.
	 */
	if (result != 0 && result != 256)
	{
		ERROR_MESSAGE("Failed to zip/unzip [%s] [%d]", destination.c_str(), result);
		return false;
	}

	return true;
}

//=========================================================================

std::string
Filesystem::GetFilenameFromPath(
		const std::string& filename)
{
	size_t found = filename.find_last_of("/\\");

	if (std::string::npos == found)
	{
		return filename;
	}

	return filename.substr(found + 1, filename.size());
}

//=========================================================================

std::string
Filesystem::GetFilenameFromPathRemoveExtension(
		const std::string& filename)
{
	auto filenameCleaned = GetFilenameFromPath(filename);
	size_t found = filenameCleaned.find_last_of(".");

	if (std::string::npos == found)
	{
		return filenameCleaned;
	}

	return filenameCleaned.substr(0, found);

}

//=========================================================================

std::string
Filesystem::GetFilenameExtension(
		const std::string& filename)
{
	auto filenameCleaned = GetFilenameFromPath(filename);
	size_t found = filenameCleaned.find_last_of(".");

	if (std::string::npos == found)
	{
		return std::string();
	}

	return filenameCleaned.substr(found);
}

//=========================================================================

bool
Filesystem::Same(
		const std::string& filename1,
		const std::string& filename2)
{
	if (filename1.empty() == true ||
		filename2.empty() == true)
	{
		GENERIC_EXCEPTION("file names cannot be empty");
	}

	struct stat buffer1 = {0};
	auto result1 = stat(filename1.c_str(), &buffer1);

	struct stat buffer2 = {0};
	auto result2 = stat(filename2.c_str(), &buffer2);

	/*
	 * Check if they both exists
	 */
	if (result1 != 0 || result2 != 0)
	{
		return false;
	}

	/*
	 * Check for same device and for same inode
	 */
	if (buffer1.st_dev == buffer2.st_dev &&
		buffer1.st_ino == buffer2.st_ino)
	{
		return true;
	}

	return false;
}

//=========================================================================

bool
Filesystem::ShuffleFile(
		const std::string& inFile,
		const std::string& outFile)
{
	std::string cmd = "shuf " +  inFile + " --output=" + outFile;
	int result = std::system(cmd.c_str());

	if (result == 0)
	{
		return true;
	}

	return false;
}

//=========================================================================

bool
Filesystem::ShuffleFile(
		const std::string& file)
{
	return ShuffleFile(file, file);
}

//=========================================================================

bool
Filesystem::EndsWith(
		const std::string& file,
		const std::string& pattern)
{
	if (FileExists(file) == false)
	{
		return false;
	}

	std::regex fileMatcher(pattern);

	return pattern.empty() ||
			std::regex_search(file, fileMatcher);
}

//=========================================================================

bool
Filesystem::IsCSV(
		const std::string& file)
{
	return EndsWith(file, CSV_FILES_REGEX);
}

//=========================================================================

bool
Filesystem::IsJson(
		const std::string& file)
{
	return EndsWith(file, JSON_FILES_REGEX);
}

//=========================================================================
