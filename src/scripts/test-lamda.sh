#!/bin/bash

LAMBDA_URL="http://localhost:9000/2015-03-31/functions/function/invocations"

GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${BLUE}Testing Evalr Lambda Function${NC}\n"

# Function to invoke Lambda
invoke_lambda() {
    local path=$1
    local method=$2
    local body=$3
    
    echo -e "${GREEN}Testing: ${method} ${path}${NC}"
    
    local payload=$(cat <<EOF
{
  "path": "${path}",
  "httpMethod": "${method}",
  "headers": {
    "Content-Type": "application/json"
  },
  "body": ${body}
}
EOF
)
    
    echo "Request:"
    echo "$payload" | jq '.' 2>/dev/null || echo "$payload"
    echo ""
    
    echo "Response:"
    curl -s -X POST "${LAMBDA_URL}" \
        -H "Content-Type: application/json" \
        -d "${payload}" | jq '.' 2>/dev/null || echo "Error: jq not installed or invalid JSON"
    echo -e "\n---\n"
}

# Test cases
case "${1:-all}" in
    health)
        invoke_lambda "/health" "GET" "null"
        ;;
    
    evaluate)
        # Arithmetic
        invoke_lambda "/evaluate" "POST" '"{\"expression\":\"2 + 3 * 4\"}"'
        
        # Variables
        invoke_lambda "/evaluate" "POST" '"{\"expression\":\"x^2 + y\",\"variables\":{\"x\":5,\"y\":3}}"'
        
        # Boolean expression
        invoke_lambda "/evaluate" "POST" '"{\"expression\":\"5 > 3 and 2 < 4\"}"'
        ;;
    
    extract)
        invoke_lambda "/extract-variables" "POST" '"{\"expression\":\"a + b * c\"}"'
        ;;
    
    error)
        # Missing operator
        invoke_lambda "/evaluate" "POST" '"{\"expression\":\"2 3\"}"'
        
        # Division by zero
        invoke_lambda "/evaluate" "POST" '"{\"expression\":\"5 / 0\"}"'
        
        # Missing variable
        invoke_lambda "/evaluate" "POST" '"{\"expression\":\"x + 5\"}"'
        ;;
    
    all|*)
        echo -e "${BLUE}Running all tests...${NC}\n"
        $0 health
        $0 evaluate
        $0 extract
        echo -e "${RED}Testing error cases...${NC}\n"
        $0 error
        ;;
esac

echo -e "${GREEN}Testing complete!${NC}"