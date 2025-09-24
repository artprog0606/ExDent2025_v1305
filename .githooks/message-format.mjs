#!/usr/bin/env node

import {readFileSync, writeFileSync } from 'node:fs';
import {validateMessage} from './message-format.core.mjs';

const message = readFileSync(process.argv[2], 'utf8').trim();
const {errorMessages, messageModified} = validateMessage(message);

if (errorMessages?.length > 0) {
    console.info("\r\n")
    console.error("Invalid format message:");
    console.error(errorMessages.map((item, index) => `${index + 1}) ${item}`).join('\r\n'))
    console.info("========")
    console.info("Example:\r\nfix(client): add product in cart\r\nhttps://task.advant.su/adminv3/projects/1?modal=2")
    console.info("========")
    console.info("\r\n")
    process.exit(1);
}else{
    writeFileSync(process.argv[2], messageModified);
    process.exit(0);
}