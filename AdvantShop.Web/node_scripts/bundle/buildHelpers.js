import { readFileSync } from 'node:fs';
import { getPathPages, projectsPathData } from '../shopPath.js';
import { existsSync } from 'node:fs';
import path from 'node:path';
import { execSync } from 'node:child_process';

export const getEnvParamsForConfig = async (project, directoryWork) => {
    const projectPath = projectsPathData.get(project);
    const _directoryWork = directoryWork ?? projectPath.getPhysicalPath();

    const subProjectPath = projectsPathData.get(project).createPathData(_directoryWork);
    const publicPath = subProjectPath.getPublicPath();

    const pages = await getPathPages(_directoryWork);
    const entryPoints = pages.getList();

    return { entryPoints, directoryWork: _directoryWork, publicPath, project, projectPath: subProjectPath };
};

export const installPackagesFromCustomPackageJson = (workingDir) => {
    if (existsSync(path.join(workingDir, `package.json`))) {
        execSync(`npm i`, { cwd: workingDir });
    }
};

export const getJsonFileSync = (filePath) => {
    const fileUrl = new URL(filePath, import.meta.url);
    return JSON.parse(readFileSync(fileUrl, 'utf8'));
};
