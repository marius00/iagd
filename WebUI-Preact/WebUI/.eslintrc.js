module.exports = {
    "rules": {
        "defaultSeverity": "warn",
        "ban": "off",
        "class-name": "warn",
        "comment-format": [
            "off",
            "check-space"
        ],
        "curly": "warn",
        "eofline": "off",
        "forin": "warn",
        "indent": [
            "warn", 2
        ],
        "interface-name": [
            "off",
            "never-prefix"
        ],
        "jsdoc-format": "warn",
        "jsx-no-lambda": "off",
        "jsx-no-multiline-js": "off",
        "react/jsx-tag-spacing": "off",
        "@typescript-eslint/no-empty-function": "off",
        "label-position": "warn",
        "@typescript-eslint/interface-name-prefix": "off",
        "max-line-length": [
            "off",
            170
        ],
        "member-ordering": [
            "off",
            "public-before-private",
            "static-before-instance",
            "variables-before-functions"
        ],
        "no-any": "off",
        "no-arg": "warn",
        "no-bitwise": "warn",
        "no-console": [
            "off",
            "log",
            "error",
            "debug",
            "info",
            "time",
            "timeEnd",
            "trace"
        ],
        "no-consecutive-blank-lines": "off",
        "no-construct": "warn",
        "no-debugger": "warn",
        "no-duplicate-variable": "warn",
        "no-empty": "warn",
        "no-eval": "warn",
        "no-shadowed-variable": "warn",
        "no-string-literal": "warn",
        "no-switch-case-fall-through": "warn",
        "no-trailing-whitespace": "off",
        "no-unused-expression": "warn",
        "no-use-before-declare": "warn",
        "one-line": [
            "warn",
            "check-catch",
            //"check-else",
            "check-open-brace",
            "check-whitespace"
        ],
        "quotemark": [
            "warn",
            "single",
            "jsx-double"
        ],
        "radix": "warn",
        "semicolon": [
            "warn",
            "always"
        ],
        "switch-default": "warn",
        "trailing-comma": [
            "off"
        ],
        "triple-equals": [
            "warn",
            "allow-null-check"
        ],
        "typedef": [
            "warn",
            "parameter",
            "property-declaration"
        ],
        "typedef-whitespace": [
            "warn",
            {
                "call-signature": "nospace",
                "index-signature": "nospace",
                "parameter": "nospace",
                "property-declaration": "nospace",
                "variable-declaration": "nospace"
            }
        ],
        "variable-name": [
            "warn",
            "ban-keywords",
            "check-format",
            "allow-leading-underscore",
            "allow-pascal-case"
        ],
        "whitespace": [
            "warn",
            "check-branch",
            "check-decl",
            "check-module",
            "check-operator",
            "check-separator",
            "check-type",
            "check-typecast"
        ]
    }
}