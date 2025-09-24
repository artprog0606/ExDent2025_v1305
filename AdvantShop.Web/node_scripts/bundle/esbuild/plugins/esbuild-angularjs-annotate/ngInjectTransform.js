import { parse } from '@babel/parser';
import traverse from '@babel/traverse';
import * as babelTypes from '@babel/types';
import generate from '@babel/generator';

const getNgInjectComment = (node) =>
    node?.leadingComments !== undefined && node?.leadingComments !== null ? node.leadingComments.find((x) => x.value.includes('@ngInject')) : null;
const createNgInject = (name, args) =>
    babelTypes.expressionStatement(
        babelTypes.assignmentExpression(
            '=',
            babelTypes.memberExpression(babelTypes.identifier(name), babelTypes.identifier('$inject')),
            babelTypes.arrayExpression(args),
        ),
    );

const createNewName = () => `anFn_${new Date().getTime().toString().substring(0, 5)}`;

export const ngInjectTransform = (code, isTypescript) => {
    const ast = parse(code, {
        sourceType: 'module',
        plugins: isTypescript ? ['typescript'] : [],
    });
    traverse.default(ast, {
        ClassMethod(path) {
            if (path.node.kind !== 'constructor') {
                return;
            }
            const classParentPath = path.findParent((x) => x.type === 'ClassDeclaration');

            const comment = getNgInjectComment(path.node) || getNgInjectComment(classParentPath.node);
            // ngInject for class method or class
            if (comment !== undefined && comment !== null) {
                babelTypes.removeComments(path.node);
                babelTypes.removeComments(classParentPath.node);

                const args = [];

                for (const item of path.node.params) {
                    if (item.type === 'Identifier') {
                        args.push(babelTypes.stringLiteral(item.name));
                    } else {
                        args.push(babelTypes.stringLiteral(item.parameter.name));
                    }
                }

                if (args.length > 0) {
                    classParentPath.insertAfter(createNgInject(classParentPath.node.id.name, args));
                }
            }
        },
        ArrowFunctionExpression(path) {
            const variableParentPath = path.findParent((x) => x.type === 'VariableDeclaration');
            const comment = getNgInjectComment(path.node) || getNgInjectComment(variableParentPath?.node);

            if (comment !== undefined && comment !== null) {
                const args = path.node.params.map((x) => babelTypes.stringLiteral(x.name));

                if (args.length > 0) {
                    babelTypes.removeComments(path.node);

                    if (variableParentPath !== undefined && variableParentPath !== null) {
                        babelTypes.removeComments(variableParentPath.node);
                    }

                    if (variableParentPath !== undefined && variableParentPath !== null) {
                        variableParentPath.insertAfter(createNgInject(variableParentPath.node.declarations[0].id.name, args));
                    } else {
                        const clone = babelTypes.cloneNode(path.node);
                        path.replaceWith(babelTypes.arrayExpression([...args, clone]));
                    }
                }
            }
        },
        FunctionDeclaration(path) {
            const parentExport = path.findParent((x) => x.type === 'ExportDefaultDeclaration' || x.type === 'ExportNamedDeclaration');
            const comment = getNgInjectComment(path.node) ?? getNgInjectComment(parentExport?.node);

            if (comment !== undefined && comment !== null) {
                babelTypes.removeComments(path.node);

                if (parentExport !== undefined && parentExport !== null) {
                    babelTypes.removeComments(parentExport.node);
                }

                const args = path.node.params.map((x) => babelTypes.stringLiteral(x.name));

                if (args.length > 0) {
                    if (path.node?.id?.name === undefined || path.node?.id?.name === null) {
                        path.node.id = babelTypes.identifier(createNewName());
                    }
                    path.insertAfter(createNgInject(path.node.id.name, args));
                }
            }
        },
        FunctionExpression(path) {
            const variableParentPath = path.findParent((x) => x.type === 'VariableDeclaration' || x.type === 'ObjectProperty');
            const exportNamedPath = path.findParent((x) => x.type === 'ExportNamedDeclaration');

            const comment =
                getNgInjectComment(path.node) || getNgInjectComment(variableParentPath?.node) || getNgInjectComment(exportNamedPath?.node);

            if (comment !== undefined && comment !== null) {
                const args = path.node.params.map((x) => babelTypes.stringLiteral(x.name));

                if (args.length > 0) {
                    babelTypes.removeComments(path.node);

                    if (variableParentPath !== undefined && variableParentPath !== null) {
                        babelTypes.removeComments(variableParentPath.node);
                    }

                    if (exportNamedPath !== undefined && exportNamedPath !== null) {
                        babelTypes.removeComments(exportNamedPath.node);
                    }

                    if (variableParentPath !== undefined && variableParentPath !== null && variableParentPath.type !== 'ObjectProperty') {
                        variableParentPath.insertAfter(createNgInject(variableParentPath.node.declarations[0].id.name, args));
                    } else {
                        const clone = babelTypes.cloneNode(path.node);
                        path.replaceWith(babelTypes.arrayExpression([...args, clone]));
                    }
                }
            }
        },
        ObjectMethod(path) {
            const comment = getNgInjectComment(path.node);

            if (comment !== undefined && comment !== null) {
                const args = path.node.params.map((x) => babelTypes.stringLiteral(x.name));

                if (args.length > 0) {
                    babelTypes.removeComments(path.node);
                    const functionExpEqual = babelTypes.functionExpression(null, path.node.params, path.node.body);
                    const objectPropertyEqual = babelTypes.objectProperty(
                        babelTypes.identifier(path.node.key.name),
                        babelTypes.arrayExpression([...args, functionExpEqual]),
                    );
                    path.replaceWith(objectPropertyEqual);
                }
            }
        },
    });
    const result = generate.default(ast, {
        retainLines: false,
        sourceMaps: false,
        concise: false,
    });
    return result.code;
};
