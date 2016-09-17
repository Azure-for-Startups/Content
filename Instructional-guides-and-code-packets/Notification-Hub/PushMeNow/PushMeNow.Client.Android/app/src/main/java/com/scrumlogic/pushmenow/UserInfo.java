package com.scrumlogic.pushmenow;

import android.util.Base64;

import org.json.JSONException;
import org.json.JSONObject;

import java.nio.charset.Charset;
import java.util.UUID;

public class UserInfo {
    private String _name;
    private String _salt;

    public String getName() {
        return _name;
    }

    public void setName(String name) {
        _name = name;
        int salt_length = 7;
        do {
            _salt = UUID.randomUUID().toString().substring(0, ++salt_length);
        } while (this.getAsTag().contains("="));
    }

    public String getSalt() {
        return _salt;
    }

    public String getAsTag() {
        JSONObject json = new JSONObject();
        try {
            json.put("Name", _name);
            json.put("Salt", _salt);
            return Base64.encodeToString(json.toString().getBytes(), Base64.NO_WRAP);
        } catch (JSONException e) {
        }
        return null;
    }
}
