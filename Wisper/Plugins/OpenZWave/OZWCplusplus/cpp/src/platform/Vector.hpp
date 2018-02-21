#pragma once

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
#include <unordered_map>

#include "Macros.hpp"
#include "Constants.hpp"

namespace Yodiwo
{
	namespace Utilities
	{
		namespace Vector
		{
			//=========================================================

			template<class T>
			std::ostream& operator << (std::ostream& os, const std::tuple<T,T>& v)
			{
				os << "(" << std::get<0>(v) << ", " << std::get<1>(v) << ")";
				return os;
			}

			//=========================================================

			template<class T>
			std::ostream& operator << (std::ostream& os, const std::vector<T>& v)
			{
				os << "[";
				for (auto value : v)
				{
					os << " " << value;
				}
				os << "]";
				return os;
			}

			//=========================================================

			template<typename T>
			bool SameSize(
					const std::vector<std::vector<T>>& values)
			{
				if (values.size() == 0)
				{
					return true;
				}

				size_t size = values[0].size();

				for (auto value : values)
				{
					if (value.size() != size)
					{
						return false;
					}
				}

				return true;
			}

			//=========================================================

			template<typename T>
			std::vector<std::tuple<T,T>> MinMaxPerDimension(
					const std::vector<std::vector<T>>& values)
			{
				std::vector<std::tuple<T,T>> results;

				if (SameSize(values) == false)
				{
					ERROR_MESSAGE("Not all values are of the same size");
					return results;
				}

				if (values.size() == 0)
				{
					ERROR_MESSAGE("Values is empty");
					return results;
				}

				const size_t size = values.size();
				const size_t dimensions = values[0].size();

				std::vector<T> minValue(dimensions, std::numeric_limits<T>::max());
				std::vector<T> maxValue(dimensions, std::numeric_limits<T>::min());

				for (size_t i = 0 ; i < size; i++)
				{
					auto& value = values[i];

					for (size_t j = 0 ; j < dimensions; j++)
					{
						auto valueAtDim = value[j];
						minValue[j] = std::min(minValue[j], valueAtDim);
						maxValue[j] = std::max(maxValue[j], valueAtDim);
					}
				}

				for (size_t j = 0 ; j < dimensions; j++)
				{
					results.emplace_back(std::make_tuple(minValue[j], maxValue[j]));
				}

				return results;
			}

			//=========================================================

			/*
			 * ScaleOffset Vectors, Straight and reverse transform
			 * straight gets you from feature -> [0 1]
			 * reverse gets you from [0 1] -> feature
			 */
			template<typename T>
			std::vector<std::tuple<T,T>> GetScaleVectors(
					const std::vector<std::vector<T>>& values)
			{
				std::vector<std::tuple<T,T>> results;

				auto minMaxVector = MinMaxPerDimension(values);

				for (auto minMax : minMaxVector)
				{
					auto offset = std::get<0>(minMax);
					auto scale = std::max(
							std::abs(std::get<1>(minMax) - std::get<0>(minMax)),
							std::abs(std::get<0>(minMax) - std::get<1>(minMax)));
					results.emplace_back(std::make_tuple(offset, scale));
				}

				return results;
			}

			//=========================================================

			/*
			 * Scale features to (value + offset) / scale
			 */
			template<typename T>
			std::vector<std::vector<T>> ScaleVectors(
					const std::vector<std::vector<T>>& values,
					const std::vector<std::tuple<T,T>>& offsetScale)
			{
				const auto epsilon = std::numeric_limits<T>::epsilon();
				std::vector<std::vector<T>> results(values);

				for (auto& value : results)
				{
					for (size_t i = 0; i < value.size(); i++)
					{
						auto offset = std::get<0>(offsetScale[i]);
						auto scale = std::get<1>(offsetScale[i]) + epsilon;
						value[i] = (value[i] - offset) / scale;
					}
				}

				return results;
			}

			//=========================================================

			/*
			 * Scale features to (value + offset) / scale
			 */
			template<typename T>
			std::vector<T> ScaleVectors(
					const std::vector<T>& value,
					const std::vector<std::tuple<T,T>>& offsetScale)
			{
				const auto epsilon = std::numeric_limits<T>::epsilon();
				std::vector<T> results(value);

				for (size_t i = 0; i < value.size(); i++)
				{
					auto offset = std::get<0>(offsetScale[i]);
					auto scale = std::get<1>(offsetScale[i]) + epsilon;
					results[i] = (value[i] - offset) / scale;
				}

				return results;
			}

			//=========================================================

			/*
			 * Scale features to (value * scale) - offset
			 */
			template<typename T>
			std::vector<std::vector<T>> ReverseScaleVectors(
					const std::vector<std::vector<T>>& values,
					const std::vector<std::tuple<T,T>>& offsetScale)
			{
				std::vector<std::vector<T>> results(values);

				for (auto& value : results)
				{
					for (size_t i = 0; i < value.size(); i++)
					{
						auto offset = std::get<0>(offsetScale[i]);
						auto scale = std::get<1>(offsetScale[i]);
						value[i] = (value[i] * scale) + offset;
					}
				}

				return results;
			}

			//=========================================================

			/*
			 * Scale features to (value * scale) - offset
			 */
			template<typename T>
			std::vector<T> ReverseScaleVectors(
					const std::vector<T>& value,
					const std::vector<std::tuple<T,T>>& offsetScale)
			{
				std::vector<T> results(value);

				for (size_t i = 0; i < value.size(); i++)
				{
					auto offset = std::get<0>(offsetScale[i]);
					auto scale = std::get<1>(offsetScale[i]);
					results[i] = (value[i] * scale) + offset;
				}

				return results;
			}

			//=========================================================

			template<typename T>
			std::vector<T> LinSpace(
					const T start,
					const T end,
					uint32_t steps)
			{
				if (steps <= 0)
				{
					GENERIC_EXCEPTION("Steps must be greater than 0, [steps=%d]", steps);
				}

				std::vector<T> result;
				result.reserve(steps);

				T step = static_cast<T>((end - start) / steps);

				for (T i = start ; i < end ; i += step)
				{
					result.push_back(i);
				}

				return result;
			}

			//=========================================================

			template<typename T>
			std::string
			VectorToString(const std::vector<T>& values)
			{
				std::stringstream ss;
				size_t i = 0;

				for (auto value : values)
				{
					ss << "[" << i <<"]="<<value<<" ";
					i++;
				}
				return ss.str();
			}

			//=========================================================

			template<typename T, class UnaryPredicate>
			std::vector<T>
			VectorFilter(
					const std::vector<T>& original,
					UnaryPredicate pred)
			{
				std::vector<T> filtered;
				std::copy_if(begin(original), end(original),
							 std::back_inserter(filtered),
							 pred);

				return filtered;
			}

			//=========================================================

			template<typename T2, typename T1, class UnaryOperation>
			std::vector<T2>
			VectorMap(
					const std::vector<T1>& original,
					UnaryOperation mappingFunction)
			{
				std::vector<T2> mapped;
				std::transform(
						begin(original),
						end(original),
						std::back_inserter(mapped),
						mappingFunction);

				return mapped;
			}

			//=========================================================
			/*
			 * Returns, {index, value}
			 */
			template<typename T = double>
			std::tuple<size_t, T>
			VectorMaxElement(
					const std::vector<T>& values)
			{
				size_t index = 0, maxIndex = 0;
				auto maxValue = values.at(0);

				for (auto value : values)
				{
					if (value > maxValue)
					{
						maxValue = value;
						maxIndex = index;
					}

					index++;
				}

				return std::make_tuple(maxIndex, maxValue);
			}

			//=========================================================

			template<typename T>
			void
			VectorAppend(
					std::vector<T>& appendedTo,
					const std::vector<T>& appended)
			{
				appendedTo.insert(end(appendedTo), begin(appended), end(appended));
			}

			//=========================================================

			Yodiwo::VectorStats
			VectorMeanMedianStdMinMax(
					std::vector<double> v);

			//=========================================================

			Yodiwo::VectorStats
			VectorWeightedMeanMedianStdMinMax(
					std::vector<double> v,
					std::vector<double> w);

			//=========================================================

			std::tuple<
				std::vector<double>,
				std::vector<std::tuple<double,double>>,
				std::vector<size_t>>
			VectorWeightedHistogram(
					const std::vector<double>& values,
					const std::vector<double>& weights,
					const size_t noBins,
					const std::tuple<double,double> minMax);

			//=========================================================

			std::tuple<
				std::vector<double>,
				std::vector<std::tuple<double,double>>,
				std::vector<size_t>>
			VectorWeightedHistogram(
					const std::vector<double>& values,
					const std::vector<double>& weights,
					const size_t noBins);

			//=========================================================

			/*
			 * bins, ranges, indices
			 */
			std::tuple<
				std::vector<double>,
				std::vector<std::tuple<double,double>>,
				std::vector<size_t>>
			VectorHistogram(
					const std::vector<double>& values,
					const size_t noBins);

			//=========================================================

			template<typename T>
			inline bool
			ValueIsBounded(
					const T& value,
					const T& lowerBound,
					const T& highBound)
			{
				return (value >= lowerBound && value <= highBound);
			}

			//=========================================================

			template <typename T>
			std::vector<T>
			ShuffleSamples(
					const std::vector<T>& samples)
			{
				std::vector<T> shuffledData(samples);
				std::random_device rd;
				std::mt19937 g(rd());
				std::shuffle(shuffledData.begin(), shuffledData.end(), g);
				return shuffledData;
			}

			//=========================================================

			/*
			 * Get a random sample from a vector
			 */
			template <typename T>
			std::vector<T>
			RandomSample(
					const std::vector<T>& samples,
					const size_t noSamples)
			{
				if (noSamples <= 0)
				{
					GENERIC_EXCEPTION("NoSamples should be > 0");
				}

				std::vector<size_t> randomSamples(samples.size());
				std::iota(randomSamples.begin(), randomSamples.end(), 0);

			    std::random_device rd;
			    std::mt19937 gen(rd());
				std::shuffle(randomSamples.begin(), randomSamples.end(), gen);

				auto first = randomSamples.begin();
				auto last = first + std::min(noSamples, randomSamples.size());

				std::vector<T> slice;
				slice.reserve(noSamples);

				for (auto iter = first; iter != last; iter++)
				{
					slice.push_back(samples[*iter]);
				}

				return slice;
			}

			//=========================================================

			/*
			 * Get a random sample from a vector
			 */
			template <typename T>
			std::vector< std::vector<T> >
			SplitRandomSample(
					const std::vector<T>& samples,
					const size_t noSamples,
					const bool shuffle = true)
			{
				if (noSamples <= 0)
				{
					GENERIC_EXCEPTION("noSamples should be > 0");
				}

				std::vector<std::vector<T> > splits;

				if (shuffle == true)
				{
					std::vector<size_t> randomSamples(samples.size());
					std::iota(randomSamples.begin(), randomSamples.end(), 0);

					std::random_device rd;
					std::mt19937 gen(rd());
					std::shuffle(randomSamples.begin(), randomSamples.end(), gen);

					{
						auto first = randomSamples.begin();
						auto last = first + std::min(noSamples, randomSamples.size());

						std::vector<T> slice;
						slice.reserve(std::min(noSamples, randomSamples.size()));

						for (auto iter = first; iter != last; iter++)
						{
							slice.push_back(samples[*iter]);
						}

						splits.push_back(slice);
					}

					{
						auto first = randomSamples.begin() +
								std::min(noSamples+1, randomSamples.size());
						auto last = randomSamples.end();

						std::vector<T> slice;
						slice.reserve(std::min(noSamples+1, randomSamples.size()));

						for (auto iter = first; iter != last; iter++)
						{
							slice.push_back(samples[*iter]);
						}

						splits.push_back(slice);
					}
				}
				else
				{
					{
						auto first = samples.begin();
						auto last = first + std::min(noSamples, samples.size());
						std::vector<T> slice(first, last);
						splits.push_back(slice);
					}

					{
						auto first = samples.begin() + std::min(noSamples+1, samples.size());
						auto last = samples.end();
						std::vector<T> slice(first, last);
						splits.push_back(slice);
					}
				}

				return splits;
			}

			//=========================================================

			template<typename S=double>
			std::vector<S> AddVectors(
					const std::vector<S>& vec1,
					const std::vector<S>& vec2,
					const S weight1 = 1,
					const S weight2 = 1)
			{
				auto size1 = vec1.size();
				auto size2 = vec2.size();
			    auto maxSize = std::max(size1, size2);
				std::vector<S> output(maxSize, 0);
				S mul = 1.0 / (weight1 + weight2 + std::numeric_limits<S>::epsilon());

			    if (size1 != size2)
			    {
				    for (size_t i = 0; i < size1; i++)
				    {
					    output[i] = vec1[i] * weight1 * mul;
				    }

				    for (size_t i = 0; i < size2; i++)
				    {
					    output[i] += vec2[i] * weight2 * mul;
				    }
			    }
			    else
			    {
			    	auto iterOutput = output.begin();
			    	auto iterOutputEnd = output.end();
			    	auto iter1 = vec1.begin();
			    	auto iter2 = vec2.begin();

			    	for (; iterOutput != iterOutputEnd; iterOutput++, iter1++, iter2++)
			    	{
			    		*iterOutput = (*iter1 * weight1 + *iter2 * weight2) * mul;
			    	}
			    }

				return output;
			}

			//=========================================================

			bool
			PairCompare(
					const std::pair<float, int>& lhs,
					const std::pair<float, int>& rhs);

			//=========================================================

			// Return the indices of the top N values of vector v.
			std::vector<int>
			TopIndices(
					const std::vector<float>& v,
					const unsigned int N);

			//=========================================================
		};
	};
};
