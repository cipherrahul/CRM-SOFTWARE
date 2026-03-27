import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
  stages: [
    { duration: '30s', target: 20 }, // ramp up to 20 users
    { duration: '1m', target: 20 },  // stay at 20 users
    { duration: '20s', target: 0 },  // ramp down
  ],
};

export default function () {
  let res = http.get('http://localhost:5000/api/crm/leads');
  check(res, {
    'status is 200': (r) => r.status === 200,
    'transaction time < 200ms': (r) => r.timings.duration < 200,
  });
  sleep(1);
}
