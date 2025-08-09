import { spawn } from 'child_process';
import fs from 'fs';
import path from 'path';
import dotenv from 'dotenv';

let SSL_KEY_FILE, SSL_CRT_FILE;
const envPath = path.resolve(process.cwd(), '.env.development.local');
if (!fs.existsSync(envPath)) {
  console.error(`Error: ${envPath} does not exist`);
  process.exit(1);
}

try {
  const envConfig = dotenv.parse(fs.readFileSync(envPath));
  SSL_KEY_FILE = envConfig.SSL_KEY_FILE;
  SSL_CRT_FILE = envConfig.SSL_CRT_FILE;
} catch (err) {
  console.error('Error loading .env.development.local:', err);
  process.exit(1);
}

const args = ['dev', '--turbopack'];

if (SSL_KEY_FILE && SSL_CRT_FILE) {
  args.push(
    '--experimental-https',
    `--experimental-https-key=${SSL_KEY_FILE}`,
    `--experimental-https-cert=${SSL_CRT_FILE}`,
    `--experimental-https-ca=${SSL_CRT_FILE}`
  );
}

const dev = spawn('next', args, {
  stdio: 'inherit',
  shell: true
});

dev.on('exit', code => process.exit(code));
