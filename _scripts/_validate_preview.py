#!/usr/bin/env python3
# -*- coding: utf-8 -*-
# 无头校验：横向溢出 / console error / 卡数 / 落地块 / 折叠复制
import os, subprocess
NODE = r"C:/Users/Administrator/.workbuddy/binaries/node/versions/22.22.2-3/node.exe"
NODE_PATH = r"C:/Users/Administrator/.workbuddy/binaries/node/workspace/node_modules"
CHROME = r"C:/Users/Administrator/AppData/Local/ms-playwright/chromium-1234/chrome-win64/chrome.exe"
HTML = r"E:/SteamLibrary/steamapps/common/RimWorld/11/RUIF/docs/02-组件图鉴/标准规范组件预览.html"
JS = r"""
const { chromium } = require('playwright-core');
const { pathToFileURL } = require('url');
(async () => {
  const browser = await chromium.launch({ executablePath: process.env.CHROME });
  const page = await browser.newPage({ viewport: { width: 1280, height: 900 } });
  const errors = [];
  page.on('console', m => { if (m.type() === 'error') errors.push(m.text()); });
  page.on('pageerror', e => errors.push('PAGEERROR: ' + e.message));
  const url = pathToFileURL(process.argv[2]).href;
  await page.goto(url, { waitUntil: 'networkidle' });
  await page.waitForTimeout(300);
  const m = await page.evaluate(() => {
    const docW = document.documentElement.scrollWidth;
    const winW = window.innerWidth;
    return {
      comp: document.querySelectorAll('div.comp, div.comp.vanilla').length,
      stdLand: document.querySelectorAll('div.land:not(.bclass)').length,
      bLand: document.querySelectorAll('div.land.bclass').length,
      overflowX: docW - winW,
      docW, winW,
      foldBtns: document.querySelectorAll('.fold-btn').length,
      copyBtns: document.querySelectorAll('.copy-btn').length,
    };
  });
  let foldOk = 'N/A';
  if (m.foldBtns > 0) {
    const before = await page.evaluate(() => document.querySelector('div.comp .fold') ? getComputedStyle(document.querySelector('div.comp .fold')).display : 'none');
    await page.click('div.comp .fold-btn');
    await page.waitForTimeout(150);
    const after = await page.evaluate(() => document.querySelector('div.comp .fold') ? getComputedStyle(document.querySelector('div.comp .fold')).display : 'none');
    foldOk = (before !== after) ? 'OK' : ('NOCHANGE ' + before + '->' + after);
  }
  let copyOk = 'N/A';
  if (m.copyBtns > 0) {
    try { await page.click('div.comp .copy-btn'); copyOk = 'OK'; } catch(e) { copyOk = 'ERR ' + e.message; }
  }
  console.log(JSON.stringify({ ...m, foldOk, copyOk, consoleErrors: errors }, null, 2));
  await page.screenshot({ path: process.env.SHOT, fullPage: false });
  await browser.close();
})();
"""
env = dict(os.environ)
env.update({"NODE_PATH": NODE_PATH, "CHROME": CHROME,
            "SHOT": r"E:/SteamLibrary/steamapps/common/RimWorld/11/RUIF/_preview_test.png"})
jsfile = r"E:/SteamLibrary/steamapps/common/RimWorld/11/RUIF/_scripts/_val_tmp.js"
open(jsfile, "w", encoding="utf-8").write(JS)
r = subprocess.run([NODE, jsfile, HTML], env=env, capture_output=True, text=True)
print("STDOUT:", r.stdout)
print("STDERR:", r.stderr[:2000])
if os.path.exists(jsfile): os.remove(jsfile)
