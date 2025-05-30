import http from 'k6/http';
import { check, sleep } from 'k6';

const executor = 'constant-arrival-rate'; // Executor que mantém uma taxa constante de requisições
//const executor = 'constant-vus'; // Executor que mantém um número constante de VUs (Virtual Users)
const durationValue = 5; // Duração do teste em durationUnit
const rate = 20000; // Taxa de requisições por minuto
const vus = 100; // Número de VUs alocados ou pré-alocados para o teste

const timeout = '2s'; // Define o timeout para as requisições
const durationUnit = 's'; // 'm' para minutos, 's' para segundos
const duration = durationValue + durationUnit; // Converte o valor de duração para o formato correto

export const options = {
  scenarios: {
    run: {
      executor: executor,
      //vus: vus, // Número de VUs alocados para o teste usando executor 'constant-vus'
      preAllocatedVUs: vus, // Número de VUs pré-alocados para o teste
      maxVUs: vus * 10, // Número máximo de VUs que podem ser alocados durante o teste
      rate: rate, // Taxa de requisições por minuto usando executor 'constant-arrival-rate'
      timeUnit: '1m',// Define a taxa de requisições por minuto
      duration: duration, // Duração do teste
      exec: 'run', // Nome da função que será executada
    }
  },
};

let token;
//token = '...';
//cuidado com o limite da google api!!
//const url = 'https://boraapi.azurewebsites.net/events/?user=bora.work';//talo: 500RPM, 2 VUs
const url = 'https://boraapi.azurewebsites.net/accounts';//talo: 500RPM, 2 VUs

const headers = {
  'Accept': 'text/plain',
  'Authorization': `Bearer ${token}`,
};

export function run() {
  const res = http.get(url, { headers, timeout: timeout });
  console.log(`${res.status_text}: ${res.timings.duration}ms`);

  check(res, {
      'http status success': (r) => r.status >= 200 && r.status < 400,
  });

  //sleep(1); // Pausa de 1 segundo entre as requisições
}

export function handleSummary(data) {
  // Helper para converter bytes para MB
  const bytesToMB = (bytes) => (bytes / (1024 * 1024)).toFixed(2);

  const checkStatusSuccess = data.root_group.checks.find(c => c.name === 'http status success');
  const passed = checkStatusSuccess?.passes || 0;
  const failed = checkStatusSuccess?.fails || 0;
  const total = passed + failed;
  const successRate = (passed / total) * 100;
  const failureRate = (failed / total) * 100;

  const green = "\x1b[32m";
  const red = "\x1b[31m";
  const yellow = "\x1b[33m";
  const reset = "\x1b[0m";

  return {
    stdout: `
======== 📊 Resumo do teste usando ${executor} ========

✔ Requisições por status
  - ${green} Sucesso (2xx-3xx): ${successRate.toFixed(2)}% (${passed} de ${total})
  - ${red} Falhas (4xx-5xx): ${failureRate.toFixed(2)}% (${failed} de ${total})
${reset}

⚡ Taxa de Entrada | Arrival Rate (λ): ${(data.metrics.iterations.values.count / (data.state.testRunDurationMs / 1000)).toFixed(2)} req/s
✔ Vazão | Throughput (X): ${data.metrics.http_reqs.values.rate.toFixed(2)} req/s

⏱ Tempo de resposta | Response Time (R):
   - Média:  ${data.metrics.http_req_duration.values.avg.toFixed(2)} ms
   - Máximo: ${data.metrics.http_req_duration.values.max.toFixed(2)} ms
   - Mínimo: ${data.metrics.http_req_duration.values.min.toFixed(2)} ms
   - Mediana: ${data.metrics.http_req_duration.values.med.toFixed(2)} ms

👥 Usuários Virtuais | Virtual Users (λ⋅R):
   - vus:     ${data.metrics.vus.values.value}
   - vus_max: ${data.metrics.vus_max.values.max}

🔁 Iterações concluídas: ${data.metrics.iterations.values.count}

📤 Tráfego de dados:
   - Enviados:  ${bytesToMB(data.metrics.data_sent.values.count)} MB
   - Recebidos: ${bytesToMB(data.metrics.data_received.values.count)} MB

⏱️ Duração total do teste: ${(data.state.testRunDurationMs / 1000).toFixed(2)} s
`,
  };
}

/*
| Tempo de Resposta | Classificação | Comentário                                      |
| ----------------- | ------------- | ----------------------------------------------- |
| **< 1ms**         | Ultra-rápido  | Muito raro fora de memória local/cache          |
| **1–10ms**        | Excelente     | Tipicamente dentro do mesmo datacenter ou cache |
| **10–100ms**      | Bom           | Considerado rápido para APIs em rede            |
| **100–500ms**     | Aceitável     | Padrão aceitável para web APIs                  |
| **> 500ms – 1s**  | Lento         | Pode impactar UX dependendo da aplicação        |
| **> 1s**          | Ruim          | Tempo de espera visível para o usuário          |
*/
