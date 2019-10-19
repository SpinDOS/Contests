#include <algorithm>

#include <queue>
#include <unordered_map>
#include <vector>

#include <iostream>

uint16_t M, N, P, K;

using pass_t = uint16_t;
using dist_t = uint16_t;
using price_t = uint16_t;

std::vector<std::unordered_map<pass_t, dist_t>> passengerEdges, driverEdges;
std::vector<price_t> prices;

void ReadInput();
void InspectDriver(const std::unordered_map<pass_t , dist_t>& distToPassengers);
void RunDijkstra(const std::unordered_map<pass_t , dist_t>& initialDistances, std::vector<dist_t>& distances);

int main() {
    ReadInput();

    for (const auto& driver : driverEdges)
        InspectDriver(driver);

    std::cout << *std::min_element(prices.cbegin(), prices.cend()) << std::endl;
    return 0;
}

void CreateEdge(std::unordered_map<pass_t, dist_t>& edges, pass_t to, dist_t value) {
    if (value >= P)
        return;

    auto existing = edges.find(to);
    if (existing == edges.end() || existing->second > value)
        edges.insert_or_assign(to, value);
}

void ReadEdges(std::vector<std::unordered_map<pass_t, dist_t>>& edgesStore) {
    uint16_t count;
    std::cin >> count;
    for (uint16_t i = 0; i < count; i++) {
        pass_t from, to;
        dist_t r;
        std::cin >> from >> to >> r;
        CreateEdge(edgesStore[from], to, r);
    }
}

void ReadInput() {
    std::cin >> M >> N >> P >> K;

    prices = std::vector<price_t>(M, K);

    driverEdges.resize(N);
    ReadEdges(driverEdges);

    passengerEdges.resize(M);
    ReadEdges(passengerEdges);
}

void InspectDriver(const std::unordered_map<pass_t , dist_t>& distToPassengers) {
    auto distances = std::vector<dist_t>(M, P);
    RunDijkstra(distToPassengers, distances);

    for (pass_t pass = 0; pass < M; pass++) {
        auto distance = distances[pass];
        if (distance >= P)
            continue;

        auto decrease = P - distance;
        prices[pass] = prices[pass] > decrease ? prices[pass] - decrease : 1;
    }
}

void RunDijkstra(const std::unordered_map<pass_t , dist_t>& initialDistances, std::vector<dist_t>& distances) {
    using PassAndDist = std::pair<pass_t, dist_t>;
    auto compare = [](PassAndDist x, PassAndDist y) { return x.second > y.second; };

    std::priority_queue<PassAndDist, std::vector<PassAndDist>, decltype(compare)> minQueue(initialDistances.cbegin(), initialDistances.cend(), compare);

    while(!minQueue.empty()) {
        auto passengerAndDistance = minQueue.top();
        minQueue.pop();

        if (distances[passengerAndDistance.first] <= passengerAndDistance.second)
            continue;

        distances[passengerAndDistance.first] = passengerAndDistance.second;

        for (const auto& edge : passengerEdges[passengerAndDistance.first]) {
            auto potentialDist = passengerAndDistance.second + edge.second;
            if (potentialDist < distances[edge.first])
                minQueue.push({edge.first, potentialDist});
        }
    }
}