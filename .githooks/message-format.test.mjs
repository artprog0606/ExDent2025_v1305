import assert from "node:assert/strict";
import { describe, it } from "node:test";
import { validateMessage, getTaskLink, ERRORS_TEXT_LIST } from "./message-format.core.mjs";

const taskUrl = getTaskLink();

describe("Validation commit message", () => {
  describe("should return error message", () => {
    it("when commit message is undefined", () => {
      const result = validateMessage(undefined);
      assert.deepEqual(result, {
        messageModified: undefined,
        errorMessages: [ERRORS_TEXT_LIST.messageEmpty],
      });
    });

    it("when commit message is empty", () => {
      const result = validateMessage("");
      assert.deepEqual(result, {
        messageModified: "",
        errorMessages: [ERRORS_TEXT_LIST.messageEmpty],
      });
    });

    it("when commit message is full format invalid", () => {
      const result = validateMessage("invalid commit message");
      assert.deepEqual(result, {
        messageModified: "invalid commit message",
        errorMessages: [ERRORS_TEXT_LIST.type, ERRORS_TEXT_LIST.taskUrl],
      });
    });

    it("when commit message contain unknown type", () => {
      const result = validateMessage(`unsupportedType: invalid commit message ${taskUrl}`);
      assert.deepEqual(result, {
        messageModified: `unsupportedType: invalid commit message \r\n${taskUrl}`,
        errorMessages: [ERRORS_TEXT_LIST.type],
      });
    });

    it("when commit message not have description", () => {
      const result = validateMessage(`fix: ${taskUrl}`);
      assert.deepEqual(result, {
        messageModified: `fix: \r\n${taskUrl}`,
        errorMessages: [ERRORS_TEXT_LIST.description],
      });
    });

    it("when commit message not have task url", () => {
      const result = validateMessage(`fix: description`);
      assert.deepEqual(result, {
        messageModified: `fix: description`,
        errorMessages: [ERRORS_TEXT_LIST.taskUrl],
      });
    });

    it("when commit message have only type", () => {
      const result = validateMessage(`fix: `);
      assert.deepEqual(result, {
        messageModified: `fix: `,
        errorMessages: [ERRORS_TEXT_LIST.description, ERRORS_TEXT_LIST.taskUrl],
      });
    });
  });

  describe("should success verify", () => {
    it("when commit message contains text 'Merge remote-tracking branch'", () => {
      const result = validateMessage(`Merge remote-tracking branch`);
      assert.deepEqual(result, {
        messageModified: `Merge remote-tracking branch`,
        errorMessages: [],
      });
    });

    it("when commit message contains type, description and task link", () => {
      const result = validateMessage(`fix: description ${taskUrl}`);
      assert.deepEqual(result, {
        messageModified: `fix: description \r\n${taskUrl}`,
        errorMessages: [],
      });
    });

    it("when commit message contains type, scope, description and task link", () => {
      const result = validateMessage(`fix(client): description ${taskUrl}`);
      assert.deepEqual(result, {
        messageModified: `fix(client): description \r\n${taskUrl}`,
        errorMessages: [],
      });
    });

    it("when commit message contains type, scope, breaking changes, description and task link", () => {
      const result = validateMessage(`fix(client)!: description result task ${taskUrl}`);
      assert.deepEqual(result, {
        messageModified: `fix(client)!: description result task \r\n${taskUrl}`,
        errorMessages: [],
      });
    });

    it("when commit message contains line breaks", () => {
      const result = validateMessage(`fix: добавил в валидацию гит-сообщения стандартный текст при мердже + рефакторинг тестов (упростил логику, так как с регулярным выражением не справился) https://task.advant.su/adminv3/projects/150?modal=38935`);
      assert.deepEqual(result, {
        messageModified: 'fix: добавил в валидацию гит-сообщения стандартный текст при мердже + рефакторинг тестов (упростил логику, так как с регулярным выражением не справился) \r\nhttps://task.advant.su/adminv3/projects/150?modal=38935',
        errorMessages: [],
      });
    });
    it("when commit message have only version", () => {
      const result = validateMessage(`--99.99.999--`);
      assert.deepEqual(result, {
        messageModified: `--99.99.999--`,
        errorMessages: [],
      });
    });

    it("when commit message have only version with white spaces", () => {
      const result = validateMessage(`-- 1.1.1 --`);
      assert.deepEqual(result, {
        messageModified: `-- 1.1.1 --`,
        errorMessages: [],
      });
    });


    it("when commit message merge master branch into local", () => {
      const result = validateMessage(`Merge branch 'master' into brunchName`);
      assert.deepEqual(result, {
        messageModified: `Merge branch 'master' into brunchName`,
        errorMessages: [],
      });
    });

    it("when commit message is multiline description", () => {
      const result = validateMessage('fix: выделенный домен админки - лишние редиректы\r\n /adminv2 -> login -> adminv2\r\n /logout - для всех \r\nhttps://task.advant.su/adminv3/tasks?modal=38513');
      assert.deepEqual(result, {
        messageModified: 'fix: выделенный домен админки - лишние редиректы\r\n /adminv2 -> login -> adminv2\r\n /logout - для всех \r\nhttps://task.advant.su/adminv3/tasks?modal=38513',
        errorMessages: [],
      });
    });

  });
});
