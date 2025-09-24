#!/usr/bin/env node

import child_process from "child_process";

const hooksPath = ".githooks";
const cmdGet = "git config core.hooksPath";
const cmdSet = `git config core.hooksPath ${hooksPath}`;
let notExist = true;

const equal = (str) => str.replaceAll(/\s/g, "") === hooksPath;

try {
  const resultGetValue = child_process.execSync(cmdGet).toString();
  notExist = equal(resultGetValue) !== true;
} catch (e) {
  notExist = true;
  console.log("githooks: couldn't get the config value");
}

if (notExist === true) {
  try {
    child_process.execSync(cmdSet).toString();
    const resultGetValue = child_process.execSync(cmdGet).toString();
    if (equal(resultGetValue) !== true) {
      throw new Error("githooks: failed to set the setting");
    }else{
        console.log("githooks: setting successfully completed");
    }
  } catch (e) {
    console.log("githooks: couldn't get the config value");
    console.log(e.message || e.stderror);
    process.exit(1);
  }
}
