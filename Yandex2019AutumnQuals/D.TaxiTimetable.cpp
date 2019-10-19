#include <algorithm>
#include <vector>

#include <iostream>

struct PeriodPrice {
    uint16_t Start, End;
    uint32_t Price;
};

std::vector<PeriodPrice> ReadInput();
uint64_t Solve(std::vector<PeriodPrice>& periodPrices);

int main() {
    auto periodPrices = ReadInput();
    std::cout << Solve(periodPrices) << std::endl;
    return 0;
}

std::vector<PeriodPrice> ReadInput() {
    uint16_t n;
    std::cin >> n;

    std::vector<PeriodPrice> result;
    result.reserve(n);

    for (uint16_t i = 0; i < n; i++) {
        PeriodPrice periodPrice;
        std::cin >> periodPrice.Start >> periodPrice.End >> periodPrice.Price;
        result.push_back(periodPrice);
    }

    return result;
}

uint64_t Solve(std::vector<PeriodPrice>& periodPrices) {
    sort(periodPrices.begin(), periodPrices.end(), [](auto x, auto y) { return x.End < y.End; });

    const auto maxTime = periodPrices.back().End;
    std::vector<uint64_t> bestPrices(maxTime + 1, 0);

    for (const auto& period : periodPrices) {
        auto alternative = bestPrices[period.Start] + period.Price;
        if (alternative <= bestPrices[period.End])
            continue;

        for (auto time = period.End; time <= maxTime; time++)
            bestPrices[time] = alternative;
    }

    return bestPrices.back();
}