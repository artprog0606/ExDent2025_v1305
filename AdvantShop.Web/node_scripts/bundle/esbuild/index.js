export const runWatch = async (watchData) => {
    console.log(`[${new Date().toLocaleTimeString()}] Start watch`);
    for (const [name, ctx] of watchData) {
        try {
            console.info(`[${new Date().toLocaleTimeString()}] Start watch: ${name}`);
            ctx.watch();
        } catch (error) {
            console.error(`[${new Date().toLocaleTimeString()}] Error: ${name} \r\n${error.message}`);
        }
    }
};
