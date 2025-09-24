import path from 'path';
import { getDirname } from './node_scripts/shopPath.js';
const __dirname = getDirname(import.meta.url);

const isCI = parseInt(process.env.CI) === 1;

/** @type {import('jest').Config} */
export default {
    preset: 'ts-jest/presets/default-esm',
    testEnvironment: 'jsdom',
    testResultsProcessor: 'jest-teamcity-reporter',
    collectCoverage: true,
    coverageReporters: ['html'],
    moduleNameMapper: {
        '\\.(css|scss)$': '<rootDir>/scripts/__test__/__mocks__/styleMock.js',
        '\\.(html)$': '<rootDir>/scripts/__test__/__mocks__/htmlMock.js',
        '\\.(svg)$': '<rootDir>/scripts/__test__/__mocks__/htmlMock.js',
        'angular-ui-bootstrap/src/tooltip/index.js': '<rootDir>/scripts/__test__/__mocks__/tooltipRequireMock.js',
        'angular-ui-bootstrap/src/popover/index.js': '<rootDir>/scripts/__test__/__mocks__/popoverRequireMock.js',
    },
    transform: {
        '^.+\\.(t|j)s$': [
            'jest-chain-transform',
            {
                transformers: [['ts-jest', { useESM: true }], 'node_scripts/jest/transform/jest-ng-transform.js'],
            },
        ],
    },
    setupFiles: ['<rootDir>/scripts/__test__/settings/setupAngularMockForJest.js', '<rootDir>/scripts/__test__/settings/setupJQ.js'],
    workerIdleMemoryLimit: isCI ? '512MB' : undefined,
};
