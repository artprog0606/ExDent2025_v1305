import path from 'path';
/**
 * Фукция для проверки возможности замены файла в шаблоне (используется в сборке вебпака)
 * @param {string} userRequest - путь, на которую надо заменить текущий
 * @returns Фукция для webpack.NormalModuleReplacementPlugin
 */

export const checkFileForReplace = (userRequest) => (resource) => {
    if (resource.context.includes('Templates') === false) {
        resource.request = path.normalize(userRequest);
    }
};
