import eslint from "@eslint/js";
import eslintConfigPrettier from 'eslint-config-prettier';
import globals from "globals";
import tseslint from 'typescript-eslint';
import angularEslint from 'eslint-plugin-angular';

const globalsVariable = {
    ...globals.browser,
    ...globals.jest,
    ...globals.jquery,
    ...globals.node,
    angular: "readonly",
    CKEDITOR: "readonly",
    ymaps: "readonly",
    CaptchaSource: "readonly",
    SmsCaptchaSource: "readonly",
    CaptchaSourcePreOrder: "readonly",
    CaptchaSourceBuyInOneClick: "readonly",
    CaptchaSourceCallback: "readonly",
    moment: "readonly",
    dragscroll: "readonly",
    SJ: "readonly",
    Sweetalert2: "readonly",
    swal: "readonly",
    doPostMessage: "readonly",
    doPostMessageWait: "readonly",
    doPostMessageDeleteCallback: "readonly",
    jsPlumb: "readonly",
    tinycolor: "readonly",
    gwClient: "readonly",
    fivepost: "readonly",
    delivery: "writable",
    SafeRouteCartWidget: "readonly",
    boxberry: "readonly",
    dataLayer: "readonly",
    ga: "readonly",
    whenAdvantshopStylesLoaded: "readonly",
    global: "readonly",
    ShiptorWidgetPvz: "readonly",
    ydwidget: "readonly",
    yd$: "readonly",
    ISDEKWidjet: "readonly",
    PickPoint: "readonly",
    psChooser: "readonly",
    flatpickr: "readonly",
    screenfull: "readonly",
} 
export default tseslint.config({
        ignores: [
            "node_modules",
            "**/vendors",
            "**/dist",
            "**/combine",
            "**/userfiles",
            "Templates",
            "Content",
            "Modules",
            "**/bootstrap.scss",
        ],
    },
    eslint.configs.all,
    eslintConfigPrettier,
    ...tseslint.configs.recommended,
    ...tseslint.configs.strict,
    ...tseslint.configs.stylistic,
    {
        plugins: {
            angularEslint
        }
    },
    {
        files: ['**/*.{ts}'],
        plugins: tseslint.plugin,
        languageOptions: {
            parser: tseslint.parser,
            parserOptions: {
                ecmaVersion: 'latest',
                project: 'tsconfig.json',
                sourceType: 'module',
            },
        },
    },
    {
        rules: {
            'sort-keys': 'off',
            'sort-vars': 'off',
            'sort-imports': 'off',
            'one-var': 'off',
            'no-magic-numbers': 'off',
            'func-style': 'off',
            'no-ternary': 'off',
            'no-continue': 'off',
            'no-nested-ternary': 'off',
            'max-lines-per-function': 'off',
            'init-declarations': 'off',
            'no-negated-condition': 'off',
            'max-statements': 'off',
            'no-use-before-define': 'off',
            'no-underscore-dangle': 'off',
            'capitalized-comments': 'off',
            'new-cap': 'off',
            "no-undefined": "off",
            'id-length': [
                'error',
                {
                    exceptions: ['_', "x", "y", "i", "j"],
                },
            ],
            "max-params": ["error", {
                "max": 4
            }]
        },
        languageOptions: {
            globals: globalsVariable,
        },
    },
    {
        files: ["**/*.js"],
        rules: {
            "no-unused-vars": "off",
            "no-prototype-builtins": "off",
            "no-useless-escape": "off",
        }
    },
    {
        files: ["**/*.{spec,test}.{js,ts}"],
        rules: {
            "no-undef": "off",
            "max-lines": "off",
        }
    })