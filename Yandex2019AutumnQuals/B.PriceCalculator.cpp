#include <memory>
#include <sstream>
#include <string>

#include <stack>
#include <vector>

#include <iostream>

using operand_t = int64_t;

struct IOperator {
    virtual operand_t Perform(const operand_t* vars) const = 0;
    virtual ~IOperator() = default;
};

using IOperatorPtr = const IOperator*;

class Calculator {
    IOperatorPtr _expressionTree;
    std::vector<std::unique_ptr<IOperator>> _allOperations;
public:
    explicit Calculator(const std::string& formula);
    operand_t Calculate(const operand_t* vars);
};

int main() {
    std::string kStr, formula, nStr;

    getline(std::cin, kStr);
    getline(std::cin, formula);
    getline(std::cin, nStr);

    auto k = std::stoul(kStr);
    auto calculator = Calculator(formula);
    auto n = std::stoul(nStr);

    operand_t vars[26];
    for (uint32_t i = 0; i < n; i++) {
        for (uint32_t varIndex = 0; varIndex < k; varIndex++)
            std::cin >> vars[varIndex];

        std::cout << calculator.Calculate(vars) << '\n';
    }

    std::cout << std::flush;
    return 0;
}

template <typename Op>
class BinaryOp final : public IOperator {
    IOperatorPtr _left, _right;
    Op _op;
public:
    explicit BinaryOp(IOperatorPtr left, IOperatorPtr right, Op op) :
        _left(left), _right(right), _op(op) { }

    operand_t Perform(const operand_t* vars) const override {
        return _op(_left->Perform(vars), _right->Perform(vars));
    }
};

class TernaryOp final : public IOperator {
    IOperatorPtr _cond, _ifTrue, _ifFalse;
public:
    explicit TernaryOp(IOperatorPtr cond, IOperatorPtr ifTrue, IOperatorPtr ifFalse) :
            _cond(cond), _ifTrue(ifTrue), _ifFalse(ifFalse) { }

    operand_t Perform(const operand_t* vars) const override {
        return _cond->Perform(vars)
            ? _ifTrue->Perform(vars)
            : _ifFalse->Perform(vars);
    }
};

class VarOp final : public IOperator {
private:
    uint8_t _varIndex;
public:
    explicit VarOp(uint8_t varIndex) : _varIndex(varIndex) { }

    operand_t Perform(const operand_t* vars) const override {
        return vars[_varIndex];
    }
};

class ConstOp final : public IOperator {
    operand_t _value;
public:
    explicit ConstOp(operand_t value) : _value(value) { }
    operand_t Perform(const operand_t* vars) const override {
        return _value;
    }
};

IOperatorPtr Pop(std::stack<IOperatorPtr>& evaluationStack) {
    auto result = evaluationStack.top();
    evaluationStack.pop();
    return result;
}

template <typename Op>
IOperator* MakeBinaryOp(std::stack<IOperatorPtr>& evaluationStack, Op op) {
    auto right = Pop(evaluationStack);
    auto left = Pop(evaluationStack);
    return new BinaryOp<Op>(left, right, op);
}

IOperator* MakeTernary(std::stack<IOperatorPtr>& evaluationStack) {
    auto ifFalse = Pop(evaluationStack);
    auto ifTrue = Pop(evaluationStack);
    auto condition = Pop(evaluationStack);
    return new TernaryOp(condition, ifTrue, ifFalse);
}

IOperator* MakeVar(char varName) {
    return new VarOp(varName - 'a');
}

IOperator* MakeConst(const std::string& token) {
    return new ConstOp(std::stoll(token));
}

IOperator* ParseToken(const std::string& token, std::stack<IOperatorPtr>& evaluationStack) {
    if (token.length() > 1)
        return MakeConst(token);

    auto firstChar = token[0];

    switch (firstChar) {
        case '+':
            return MakeBinaryOp(evaluationStack, std::plus<>());
        case '-':
            return MakeBinaryOp(evaluationStack, std::minus<>());
        case '*':
            return MakeBinaryOp(evaluationStack, std::multiplies<>());
        case '/':
            return MakeBinaryOp(evaluationStack, std::divides<>());
        case '<':
            return MakeBinaryOp(evaluationStack, std::less<>());
        case '=':
            return MakeBinaryOp(evaluationStack, std::equal_to<>());
        case '?':
            return MakeTernary(evaluationStack);
        default:
            break;
    }

    if (firstChar >= 'a' && firstChar <= 'z')
        return MakeVar(firstChar);
    else
        return MakeConst(token);
}

Calculator::Calculator(const std::string& formula) {
    std::stringstream formulaStream(formula);
    std::string token;

    std::stack<IOperatorPtr> evaluationStack;

    while (std::getline(formulaStream, token, ' ')) {
        if (token.empty())
            continue;

        auto operation = ParseToken(token, evaluationStack);
        evaluationStack.push(operation);
        _allOperations.push_back(std::unique_ptr<IOperator>(operation));
    }

    _expressionTree = evaluationStack.top();
}

operand_t Calculator::Calculate(const operand_t* vars) {
    return _expressionTree->Perform(vars);
}

