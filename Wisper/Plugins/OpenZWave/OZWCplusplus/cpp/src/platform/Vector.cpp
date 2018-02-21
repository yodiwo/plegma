#include "Vector.hpp"

using namespace Yodiwo::Utilities;

bool
Vector::PairCompare(
		const std::pair<float, int>& lhs,
		const std::pair<float, int>& rhs)
{
    return lhs.first > rhs.first;
}

// Return the indices of the top N values of vector v.
std::vector<int>
Vector::TopIndices(
		const std::vector<float>& v,
		const unsigned int N)
{
    std::vector<std::pair<float, int> > pairs;

    for (size_t i = 0; i < v.size(); i++)
    {
        pairs.push_back(std::make_pair(v[i], i));
    }

    std::partial_sort(
    		pairs.begin(),
			pairs.begin() + N, pairs.end(),
			PairCompare);

    std::vector<int> result;

    for (unsigned int i = 0; i < N; i++)
    {
        result.push_back(pairs[i].second);
    }

    return result;
}

Yodiwo::VectorStats
Vector::VectorMeanMedianStdMinMax(
		std::vector<double> v)
{
	std::vector<double> w(v.size(), 1);
	return VectorWeightedMeanMedianStdMinMax(v, w);
}

Yodiwo::VectorStats
Vector::VectorWeightedMeanMedianStdMinMax(
		std::vector<double> v,
		std::vector<double> w)
{
	const double epsilon = std::numeric_limits<double>::epsilon();
	double sumWeights = std::accumulate(w.begin(), w.end(), 0.0);
	double WeightedSum = std::inner_product(v.begin(), v.end(), w.begin(), 0.0);
	double WeightedMean = WeightedSum / (sumWeights + epsilon);
	double WeightedSquareSum = 0;

	for (size_t i = 0 ; i < v.size(); i++)
	{
		WeightedSquareSum += std::pow(v[i] - WeightedMean, 2) * w[i];
	}

	double WeightedStdDev = std::sqrt(WeightedMean / (sumWeights + epsilon));

	size_t n_thElement = size_t(std::round(double(v.size() * 0.5)));
	n_thElement = std::min(n_thElement, v.size()-1);
	n_thElement = std::max(n_thElement, size_t(0));
	std::nth_element(v.begin(), v.begin() + n_thElement, v.end());
	double median = v[n_thElement];
	auto minmax = std::minmax_element(v.begin(), v.end());
	double min = v[(minmax.first - v.begin())];
	double max = v[(minmax.second - v.begin())];
	return std::make_tuple(WeightedMean, median, WeightedStdDev, min, max);
}

std::tuple<
	std::vector<double>,
	std::vector<std::tuple<double,double>>,
	std::vector<size_t>>
Vector::VectorWeightedHistogram(
		const std::vector<double>& values,
		const std::vector<double>& weights,
		const size_t noBins,
		const std::tuple<double,double> minMax)
{
	const auto min = std::get<0>(minMax);
	const auto max = std::get<1>(minMax);
	const auto range = (noBins - 1) / (max - min);
	const auto rangeBin = (max - min) / noBins;

	std::vector<double> bins(noBins, 0);
	std::vector<size_t> indices(values.size(), 0);

	std::vector<std::tuple<double,double>> ranges;

	for (size_t i = 0; i < noBins; i++)
	{
		double rangeMin = min + rangeBin * i;
		double rangeMax = rangeMin + rangeBin;
		ranges.push_back(std::make_tuple(rangeMin, rangeMax));
	}

	for (size_t i = 0; i < values.size(); i++)
	{
		double weight = weights[i];

		if (weight == 0)
		{
			continue;
		}

		auto index = (size_t)std::round(range * (values[i] - min));

		if (index > noBins - 1)
		{
			index = noBins - 1;
		}

		if (index < 0)
		{
			index = 0;
		}

		indices[i] = index;
		bins[index] += weight;
	}

	return std::make_tuple(bins, ranges, indices);
}

std::tuple<
	std::vector<double>,
	std::vector<std::tuple<double,double>>,
	std::vector<size_t>>
Vector::VectorWeightedHistogram(
		const std::vector<double>& values,
		const std::vector<double>& weights,
		const size_t noBins)
{
	auto stats = VectorMeanMedianStdMinMax(values);
	const auto min = std::get<3>(stats);
	const auto max = std::get<4>(stats);
	return VectorWeightedHistogram(
			values, weights, noBins, std::make_tuple(min, max));
}

/*
 * bins, ranges, indices
 */
std::tuple<
	std::vector<double>,
	std::vector<std::tuple<double,double>>,
	std::vector<size_t>>
Vector::VectorHistogram(
		const std::vector<double>& values,
		const size_t noBins)
{
	std::vector<double> weights(values.size(), 1.0);
	return VectorWeightedHistogram(values, weights, noBins);
}
