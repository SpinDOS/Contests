#include <algorithm>
#include <queue>
#include <vector>

#include <iostream>

uint16_t N;

using DestinationPrice = std::pair<uint16_t, uint64_t>;
std::vector<DestinationPrice> buildPrices;
std::vector<std::vector<DestinationPrice>> pathPrices;

void ReadInput();
uint64_t Solve();

int main() {
    ReadInput();
    std::cout << Solve() << std::endl;
    return 0;
}


void ReadInput() {
    std::cin >> N;

    buildPrices.reserve(N);
    for (uint16_t i = 0; i < N; i++) {
        uint64_t price;
        std::cin >> price;
        buildPrices.push_back({i, price});
    }

    uint32_t k;
    std::cin >> k;

    pathPrices.resize(N);
    for (uint32_t i = 0; i < k; i++) {
        uint64_t x, y, price;
        std::cin >> x >> y >> price;
        --x;
        --y;

        pathPrices[x].push_back({y, price});
        pathPrices[y].push_back({x, price});
    }
}

struct ComparePrices {
    bool operator()(DestinationPrice x, DestinationPrice y) {
        return x.second > y.second;
    }
};

uint64_t Solve() {
    auto empowered = std::vector<bool>(N, false);

    std::priority_queue<DestinationPrice, std::vector<DestinationPrice>, ComparePrices> minPriceQueue(buildPrices.cbegin(), buildPrices.cend());

    uint64_t result = 0;

    while (!minPriceQueue.empty()) {
        auto withMinPrice = minPriceQueue.top();
        minPriceQueue.pop();

        if (empowered[withMinPrice.first])
            continue;

        empowered[withMinPrice.first] = true;
        result += withMinPrice.second;

        for (const auto& pair : pathPrices[withMinPrice.first])
            minPriceQueue.push(pair);
    }

    return result;
}