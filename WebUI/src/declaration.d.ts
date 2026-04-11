/// <reference types="vite/client" />

declare module "*.css" {
    const mapping: Record<string, string>;
    export default mapping;
}
