import http from 'k6/http';
import { check, sleep } from 'k6';

// Configuration: How the test runs
export const options = {
    stages: [
        // 1. Ramp up to 10 concurrent users over 30 seconds
        { duration: '30s', target: 10 },
        // 2. Ramp up to 100 concurrent users over 1 minute (Heavy Load)
        { duration: '1m', target: 100 },
        // 3. Stay at 100 users for 30 seconds to ensure stability
        { duration: '30s', target: 100 },
        // 4. Ramp down to 0 users (Cleanup)
        { duration: '30s', target: 0 },
    ],
    // Define thresholds to mark the test as Failed if performance is bad
    thresholds: {
        http_req_failed: ['rate<0.01'], // Failure rate must be less than 1%
        http_req_duration: ['p(95)<500'], // 95% of requests must be faster than 500ms
    },
};

// The actual user behavior simulation
export default function () {
    // Replace this with your actual API endpoint
    const res = http.get('http://localhost:5000/api/movies');

    // Check if the response was successful (Status 200)
    check(res, { 'status was 200': (r) => r.status == 200 });

    // Simulate a user "thinking" or reading before clicking again
    // Random sleep between 0.1 and 1 second
    sleep(1);
}