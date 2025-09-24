const whiteList = [/^Merge remote-tracking branch/u, /^--.+--$/u, /^Merge branch/u]

const taskUrl = "https://project.advant.su";
//https://www.conventionalcommits.org/en/v1.0.0
const typeList = ["fix", "feat", "del", "build", "chore", "ci", "docs", "style", "refactor", "perf", "test"];
const scopeRegexp = /\(\S+\)/u;
const descriptionRegexp = `(?<description>[\\s\\S]+)`;
const taskUrlRegexp = `(?<taskUrl>https?:\\/\\/(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\\.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]\\S+)`;
export const ERRORS_TEXT_LIST = {
    'messageEmpty': 'Commit message is null',
    'type': `Not found or invalid type\r\nAvalable types: ${typeList.join(', ')}`,
    'scope': 'Scope is string empty',
    'description': 'Description less 4 symbols',
    'taskUrl': `Not found task link (domain ${taskUrl})`
}

export const getTaskLink = () => taskUrl;

export const validateMessage = (message) => {
    const errorMessages = [];
    let messageModified = message;
    let stringIndex = 0;

    if (messageModified == null || messageModified.length === 0) {
        errorMessages.push(ERRORS_TEXT_LIST.messageEmpty);
        return {
            errorMessages,
            messageModified
        }
    }

    if (whiteList.some(item => item.test(messageModified))) {
        return {
            errorMessages,
            messageModified
        };
    }

    const type = typeList.find(type => messageModified.startsWith(type));
    if (type == null) {
        errorMessages.push(ERRORS_TEXT_LIST.type);
    } else {
        stringIndex += type.length;
    }
    messageModified = messageModified.replace(new RegExp(`^${type}\\s+`, 'u'), type);
    const scope = scopeRegexp.exec(messageModified);

    if (scope != null && scope.index === 0 && scope[0].length === 2) {
        errorMessages.push(ERRORS_TEXT_LIST.scope);
    }

    if (scope != null) {
        stringIndex += scope[0].length;
    }

    messageModified = messageModified.substring(0, stringIndex) + messageModified.substring(stringIndex).replace(/^\s+/u, '')

    if (messageModified[stringIndex] === '!') {
        stringIndex += 1;
    }

    if (messageModified[stringIndex] === ':') {
        stringIndex += 1;
    }
    messageModified = messageModified.substring(0, stringIndex) + messageModified.substring(stringIndex).replace(/^\s+/u, ' ')

    if (/^.{0,3}$/u.test(messageModified.substring(stringIndex).replace(new RegExp(`(${taskUrlRegexp}\\S*)$`, 'u'), ''))) {
        errorMessages.push(ERRORS_TEXT_LIST.description);
    }

    if (new RegExp(taskUrlRegexp, 'ui').test(messageModified.toLocaleLowerCase()) === false) {
        errorMessages.push(ERRORS_TEXT_LIST.taskUrl);
    } else {
        if (new RegExp(`[\r\n]+${taskUrlRegexp}\\S+$`, 'u').test(messageModified) === false) {
            messageModified = messageModified.replace(new RegExp(`(${taskUrlRegexp}\\S+)$`, 'u'), '\r\n$1');
        }
    }

    return {
        errorMessages,
        messageModified
    };
};