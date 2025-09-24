export default (name, callback) => {
    return {
        name: 'esbuild-watch-callback-plugin',
        setup: (build) => {
            build.onStart(() => {
                console.log(`[${new Date().toLocaleTimeString()}] Build start: ${name}`);
            });
            build.onEnd(async (buildResult) => {
                await callback(buildResult);
                console.log(`[${new Date().toLocaleTimeString()}] Build complete: ${name}`);
            });
        },
    };
};
