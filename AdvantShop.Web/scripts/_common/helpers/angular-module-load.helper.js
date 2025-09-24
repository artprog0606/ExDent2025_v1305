const findPattern = new Map();
findPattern.set('E', (item) => item);
findPattern.set('A', (item) => `[data-${item}], [${item}]`);
findPattern.set('C', (item) => `.${item}`);

class AngularModuleHelper {
    load(moduleName, fnLoad) {
        angular.module(`app`).run(
            /*@ngInject*/
            ($compile, $rootScope, $ocLazyLoad, $timeout) => {
                if ($ocLazyLoad.isLoaded(moduleName) === false) {
                    fnLoad()
                        .then((module) => {
                            return $ocLazyLoad.inject(module.default).then(() => {
                                const moduleObjects = angular.module(moduleName)._invokeQueue;
                                const directivesAndComponents = [];
                                let moduleObjectsItem;

                                for (let index in moduleObjects) {
                                    moduleObjectsItem = moduleObjects[index][1];
                                    if (moduleObjectsItem === `directive` || moduleObjectsItem === `component`) {
                                        directivesAndComponents.push(moduleObjects[index][2]);
                                    }
                                }

                                const roots = this.getRoots(directivesAndComponents);
                                const elements = roots.reduce((prev, current) => {
                                    return prev.concat(
                                        this.getElements(
                                            this.toKebabCase(current[0]),
                                            typeof current[1] === 'function' ? current[1]().restrict : current[1].restrict,
                                        ),
                                    );
                                }, []);
                                //$timeout(() => this.compile(elements, $rootScope, $compile), 0);
                                this.compile(elements, $rootScope, $compile);
                            });
                        })
                        .catch((error) => {
                            console.error(error);
                        });
                }
            },
        );
    }

    getElements(name, restrict) {
        const _restrict = restrict != null ? restrict.split() : ['E', 'A', 'C'];
        let query = [];

        if (findPattern.has('M')) {
            throw new Error(`Restrict "M" not supported`);
        }

        for (let index in _restrict) {
            if (findPattern.has(_restrict[index])) {
                query.push(findPattern.get(_restrict[index])(name));
            }
        }

        return Array.from(document.querySelectorAll(query.join(',')) || []);
    }

    compile(elements, $rootScope, $compile) {
        if (elements != null && elements.length > 0) {
            let _item;
            let _scope;
            elements.forEach((el) => {
                _item = angular.element(el);
                _scope = _item.scope() || $rootScope;
                $compile(_item)(_scope.$new());
            });
        }
    }

    toKebabCase(string) {
        return string.replace(/([a-z0-9]|(?=[A-Z]))([A-Z])/g, '$1-$2').toLowerCase();
    }

    getRoots(directives) {
        const result = [];
        const nameList = directives.map((item) => item[0]);
        let item, require, isNeedParent;
        for (let index in directives) {
            item = typeof directives[index][1] === 'function' ? directives[index][1]() : directives[index][1];
            if (item.require != null) {
                require = Array.isArray(item.require) ? item.require : [item.require];
                isNeedParent = nameList.some((nameItem) => require.includes(`^${nameItem}`));
                if (isNeedParent === false) {
                    result.push(directives[index]);
                }
            } else {
                result.push(directives[index]);
            }
        }

        return result;
    }
}

export { AngularModuleHelper };
