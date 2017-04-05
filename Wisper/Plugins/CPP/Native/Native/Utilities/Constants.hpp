#pragma once

#include <tuple>
#include <vector>

#include "json.hpp"

namespace Yodiwo
{
	static constexpr auto HTTP = "http";
	static constexpr auto HTTPS = "https";
	static constexpr auto EMPTY_STRING = "";
	static constexpr auto SYSTEM_SUCCESS = 0;
	static constexpr auto PI = 3.141592653589;
	static constexpr auto CAFFE_BINARY = "caffe";
	static constexpr auto TAR_EXTENSION = ".tar";
	static constexpr auto CSV_EXTENSION = ".csv";
	static constexpr auto JPG_EXTENSION = ".jpg";
	static constexpr auto LOG_EXTENSION = ".log";
	static constexpr auto JSON_EXTENSION = ".json";
	static constexpr auto CURRENT_DIRECTORY = "./";
	static constexpr auto CSV_FILES_REGEX = "\\.(CSV|csv)";
	static constexpr auto JSON_FILES_REGEX = "\\.(JSON|json)";
	static constexpr auto CAFFE_MODEL_EXTENSION = ".caffemodel";
	static constexpr auto DEFAULT_WINDOW_NAME = "Display window";
	static constexpr auto CAFFE_SOLVER_STATE_EXTENSION = ".solverstate";
	static constexpr auto CAFFE_MODEL_FILES_REGEX = "\\.(caffemodel|CAFFEMODEL)";
	static constexpr auto CAFFE_SOLVER_STATE_FILES_REGEX = "\\.(solverstate|SOLVERSTATE)";
	static constexpr auto DEFAULT_YODIGRAM_PHOTOSERVER = "https://photoserver.yodigram.com";
	static constexpr auto INTEGER_REGEX = "\\s*[+-]?([1-9][0-9]*|0[0-7]*|0[xX][0-9a-fA-F]+)";
	static constexpr auto IMAGE_FILES_REGEX = "\\.(png|jpeg|jpg|tiff|bmp|JPG|JPEG|BMP|TIFF|GIF|PNG)";

	// Fields
	static constexpr auto TYPE_FIELD = "type";
	static constexpr auto SETTINGS_FIELD = "settings";
	static constexpr auto OPERATIONS_FIELD = "operations";

	using json = nlohmann::json;

	// Mean, Median, Std, Min, Max
	using VectorStats = std::tuple<double, double, double, double, double>;

	// Bins, Ranges, Indices
	using VectorHistogram = std::tuple<
		std::vector<double>,
		std::vector<std::tuple<double, double> >,
		std::vector<size_t> >;

	namespace CommonJsonFields
	{
		static constexpr auto X_FIELD = "x";
		static constexpr auto Y_FIELD = "y";
		static constexpr auto ID_FIELD = "ID";
		static constexpr auto NAME_FIELD = "Name";
		static constexpr auto PATH_FIELD = "Path";
		static constexpr auto WIDTH_FIELD = "width";
		static constexpr auto LABEL_FIELD = "Label";
		static constexpr auto REGION_FIELD = "Region";
		static constexpr auto HEIGHT_FIELD = "height";
		static constexpr auto PARTOF_FIELD = "PartOf";
		static constexpr auto IMAGES_FIELD = "Images";
		static constexpr auto STATUS_FIELD = "status";
		static constexpr auto RESULTS_FIELD = "results";
		static constexpr auto ENABLED_FIELD = "enabled";
		static constexpr auto BARDCODE_FIELD = "Barcode";
		static constexpr auto PRODUCTS_FIELD = "Products";
		static constexpr auto FILENAME_FIELD = "FileName";
		static constexpr auto DIRECTORY_FIELD = "Directory";
		static constexpr auto ASPECT_RATIO_FIELD = "Aspect_Ratio";
		static constexpr auto OTHER_CATEGORIES_FIELD = "OtherCategories";
	}
};
