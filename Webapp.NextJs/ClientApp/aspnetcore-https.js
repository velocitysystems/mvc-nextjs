import { existsSync } from 'fs';
import { join } from 'path';
import { spawn as _spawn } from 'child_process';

const spawn = _spawn;

const baseFolder = process.env.APPDATA !== undefined && process.env.APPDATA !== ''
    ? `${process.env.APPDATA}/ASP.NET/https`
    : `${process.env.HOME}/.aspnet/https`;

const certArg = process.argv.map(arg => arg.match('/--name=(?<value>.+)/i')).filter(Boolean)[0];
const certName = certArg ? certArg?.groups?.value : process.env.npm_package_name;

if (!certName) {
    console.error('Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<<app>> explicitly');
    process.exit(-1);
}

const moduleCertFilePath = join(baseFolder, `${certName}.pem`);
const moduleKeyFilePath = join(baseFolder, `${certName}.key`);

if (!existsSync(moduleCertFilePath) || !existsSync(moduleKeyFilePath)) {
    spawn('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        moduleCertFilePath,
        '--format',
        'Pem',
        '--no-password',
    ], { stdio: 'inherit' })
        .on('exit', (code) => process.exit(code));
}